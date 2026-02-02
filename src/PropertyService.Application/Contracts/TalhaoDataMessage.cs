namespace AgroSolutions.Contracts;

public record TalhaoDataMessage(
    Guid TalhaoId,
    string Nome,
    Guid PropriedadeId
);
