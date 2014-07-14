using System.Collections.Generic;
using System.Linq;

namespace AzureLinkboard.Domain.Validation
{
    public class ValidationResult
    {
        private readonly List<ValidationError> _errors;

        public static ValidationResult Ok { get {  return  new ValidationResult();} }

        public static ValidationResult Fail(string message)
        {
            return Fail(null, message);
        }

        public static ValidationResult Fail(string key, string message)
        {
            return new ValidationResult(key, message);
        }

        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public ValidationResult(string key, string message)
        {
            _errors = new List<ValidationError>()
            {
                new ValidationError(key, message)
            };
        }

        public IEnumerable<ValidationError> Errors
        {
            get { return _errors; }
        }

        public bool Success
        {
            get { return !Errors.Any(); }
        }

        public bool Failed { get { return Errors.Any(); } }

        public void AddError(string message)
        {
            AddError(null, message);
        }

        public void AddError(string key, string message)
        {
            _errors.Add(new ValidationError(key, message));
        }
    }

    public class ValidationResult<T> : ValidationResult
    {
        public ValidationResult() : base()
        {
            
        }

        public ValidationResult(T item)
        {
            Item = item;
        }

        public T Item { get; internal set; }
    }
}
