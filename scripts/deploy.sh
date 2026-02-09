#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
KIND_NAME="agro-dev"
NAMESPACE="property-service"
ENV_FILE="$ROOT_DIR/.env"

DB_HOST="${DB_HOST:-postgres}"
DB_PORT="${DB_PORT:-5432}"
DB_USER="${DB_USER:-docker}"
DB_PASSWORD="${DB_PASSWORD:-docker}"
DB_NAME="${DB_NAME:-PropertyService}"
RABBITMQ_HOST="${RABBITMQ_HOST:-rabbitmq-service.sensor-ingestion.svc.cluster.local}"
RABBITMQ_PORT="${RABBITMQ_PORT:-5672}"
RABBITMQ_USER="${RABBITMQ_USER:-admin}"
RABBITMQ_PASSWORD="${RABBITMQ_PASSWORD:-admin123}"
RABBITMQ_VHOST="${RABBITMQ_VHOST:-/}"
JWT_ISSUER="${JWT_ISSUER:-IdentityService}"
JWT_KEY="${JWT_KEY:-7G+H65bLToXxqzPvj7+q0oQUlxJp1WvdOU3nv3ArA1s=}"
ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"

[ -f "$ENV_FILE" ] && set -a && source "$ENV_FILE" && set +a
[[ -z "$RABBITMQ_VHOST" || "$RABBITMQ_VHOST" == *":"* || "$RABBITMQ_VHOST" == *"Program"* ]] && RABBITMQ_VHOST="/"

echo "================================================"
echo "  PropertyService - Build & Deploy (Kind)"
echo "================================================"

command -v kind >/dev/null 2>&1 || { echo "Kind não instalado."; exit 1; }
command -v kubectl >/dev/null 2>&1 || { echo "kubectl não instalado."; exit 1; }
command -v docker >/dev/null 2>&1 || { echo "Docker não instalado."; exit 1; }

if ! kind get clusters 2>/dev/null | grep -q "^${KIND_NAME}$"; then
  echo "Criando cluster Kind ${KIND_NAME}..."
  kind create cluster --config "$ROOT_DIR/k8s/kind/config.yaml"
fi

if [ -z "${SKIP_BUILD:-}" ]; then
  echo "Build e carregamento da imagem..."
  docker build -t propertyservice-api:dev "$ROOT_DIR"
  kind load docker-image propertyservice-api:dev --name "$KIND_NAME"
fi

ctx=$(kubectl config current-context 2>/dev/null)
[[ "$ctx" != *"$KIND_NAME"* ]] && { echo "Contexto Kind não está ativo."; exit 1; }

echo "Aplicando namespace..."
kubectl apply -f "$ROOT_DIR/k8s/base/namespaces/property-service.yaml"

echo "Criando secret database-config..."
kubectl create secret generic database-config -n "$NAMESPACE" \
  --from-literal=DB_HOST="$DB_HOST" \
  --from-literal=DB_PORT="$DB_PORT" \
  --from-literal=DB_USER="$DB_USER" \
  --from-literal=DB_PASSWORD="$DB_PASSWORD" \
  --from-literal=DB_NAME="$DB_NAME" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "Aplicando Postgres (PV, PVC, Deployment, Service)..."
kubectl apply -f "$ROOT_DIR/k8s/base/postgresql/pv.yaml"
kubectl apply -f "$ROOT_DIR/k8s/base/postgresql/pvc.yaml"
kubectl apply -f "$ROOT_DIR/k8s/base/postgresql/deployment.yaml"
kubectl apply -f "$ROOT_DIR/k8s/base/postgresql/service.yaml"

WAIT_TO="${WAIT_TIMEOUT:-45}"
if kubectl wait --for=condition=ready pod -l app=postgres -n "$NAMESPACE" --timeout=0s 2>/dev/null; then echo "Postgres já pronto."; else echo "Aguardando Postgres..."; kubectl wait --for=condition=ready pod -l app=postgres -n "$NAMESPACE" --timeout="${WAIT_TO}s" 2>/dev/null || sleep 5; fi

CONN="Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
echo "Criando secret propertyservice-secret..."
kubectl create secret generic propertyservice-secret -n "$NAMESPACE" \
  --from-literal=ConnectionStrings__DefaultConnection="$CONN" \
  --from-literal=RabbitMq__Host="$RABBITMQ_HOST" \
  --from-literal=RabbitMq__Port="$RABBITMQ_PORT" \
  --from-literal=RabbitMq__Username="$RABBITMQ_USER" \
  --from-literal=RabbitMq__Password="$RABBITMQ_PASSWORD" \
  --from-literal=RabbitMq__VirtualHost="$RABBITMQ_VHOST" \
  --from-literal=Jwt__Issuer="$JWT_ISSUER" \
  --from-literal=Jwt__Key="$JWT_KEY" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "Criando ConfigMap..."
kubectl create configmap propertyservice-config -n "$NAMESPACE" \
  --from-literal=ASPNETCORE_ENVIRONMENT="$ASPNETCORE_ENVIRONMENT" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "Aplicando API..."
kubectl apply -f "$ROOT_DIR/k8s/base/app/deployment.yaml"
kubectl apply -f "$ROOT_DIR/k8s/base/app/service.yaml"

echo "Aplicando observabilidade (Prometheus + Grafana)..."
kubectl apply -f "$ROOT_DIR/k8s/base/observability/prometheus"
kubectl apply -f "$ROOT_DIR/k8s/base/observability/grafana"

if kubectl wait --for=condition=ready pod -l app=propertyservice-api -n "$NAMESPACE" --timeout=0s 2>/dev/null; then echo "API já pronta."; else echo "Aguardando API..."; kubectl wait --for=condition=ready pod -l app=propertyservice-api -n "$NAMESPACE" --timeout="${WAIT_TO}s" 2>/dev/null || true; fi

echo ""
echo "Deploy concluído. Pods:"
kubectl get pods -n "$NAMESPACE"
echo ""
echo "================================================"
echo "  PropertyService - URLs e acesso"
echo "================================================"
echo ""
echo "APIs:"
echo "  Property:       http://localhost:30082/swagger"
echo ""
echo "Infra:"
echo "  Grafana:        http://localhost:30380 (admin/admin)"
echo "  Prometheus:     http://localhost:30980"
echo ""
