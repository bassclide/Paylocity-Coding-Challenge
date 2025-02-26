namespace Api.Errors;

public sealed class EmployeeAlreadyHasDependantWithRelationship()
    : ConflictError("Employee has allowed only one dependent with relationship Spouse or Domestic partner");