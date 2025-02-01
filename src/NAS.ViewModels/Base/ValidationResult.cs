namespace NAS.ViewModels.Base
{
  public class ValidationResult
  {
    public ValidationResult()
    {
      Category = ValidationCategory.None;
    }

    public ValidationResult(string message, ValidationCategory category)
    {
      Message = message;
      Category = category;
    }

    public ValidationResult(ValidationResult other)
    {
      Category = other.Category;
      Message = other.Message;
    }

    public bool IsOK => Category != ValidationCategory.Error;

    public string Message { get; }

    public ValidationCategory Category { get; }

    public ValidationResult Merge(ValidationResult other)
    {
      return Merge(this, other);
    }

    public static ValidationResult OK()
    {
      return new();
    }

    public static ValidationResult Error(string message)
    {
      return new ValidationResult(message, ValidationCategory.Error);
    }

    public static ValidationResult Warning(string message)
    {
      return new ValidationResult(message, ValidationCategory.Warning);
    }

    public static ValidationResult Merge(ValidationResult result1, ValidationResult result2)
    {
      if (result1.IsOK && result2.IsOK)
      {
        return OK();
      }

      if (result1.IsOK)
      {
        return new ValidationResult(result2);
      }
      else if (result2.IsOK)
      {
        return new ValidationResult(result1);
      }

      return new ValidationResult(result1.Message + Environment.NewLine + result2.Message, ValidationCategory.Error);
    }
  }
}
