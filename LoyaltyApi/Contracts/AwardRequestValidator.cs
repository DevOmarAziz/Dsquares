using System.Text.RegularExpressions;

namespace LoyaltyApi.Contracts
{
    public static class AwardRequestValidator
    {
        public static readonly Regex IdentifierPattern = 
            new ("^[a-zA-Z0-9_-]{1,64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static Dictionary<string, string[]> Validate(AwardRequest? request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request is null)
            {
                errors["AwardRequest"] = new[] { "The award request cannot be null or empty." };
                return errors;
            }

            ValidateIdentifier(request.SourceId, "sourceId", errors);
            ValidateIdentifier(request.ExternalTransactionId, "externalTransactionId", errors);
            ValidateIdentifier(request.WalletId, "walletId", errors);

            if (request.Points is null || request.Points <= 0 || request.Points > 1000)
                errors["points"] = new[] { "The points must be a positive integer between 1 and 1000." };

            return errors;
        }

        private static void ValidateIdentifier(string? value, string field, Dictionary<string, string[]> errors)
        {
            if (value is null || !IdentifierPattern.IsMatch(value))
                errors[field] = new[] { $"The {field} must be a non-empty alphanumeric string (including underscores and hyphens) with a maximum length of 64 characters." };
        }
    }
}
