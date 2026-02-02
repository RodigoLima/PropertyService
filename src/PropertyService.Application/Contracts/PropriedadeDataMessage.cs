namespace AgroSolutions.Contracts;

public record PropriedadeDataMessage(
    Guid PropriedadeId,
    string Nome,
    Guid ProdutorId
);
