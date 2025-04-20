using backend.Services.Interfaces;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services.Implement
{
    public class PaymentService 
    {
        public static bool IsValidData(object dataObject, string currentSignature, string checksumKey)
        {
            var flatDict = FlattenObject(dataObject);
            var sortedDict = flatDict.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var queryString = ConvertObjToQueryStr(sortedDict);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return signature == currentSignature;
            }
        }

        private static Dictionary<string, string> FlattenObject(object obj, string prefix = "")
        {
            var result = new Dictionary<string, string>();
            if (obj == null) return result;

            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                var value = prop.GetValue(obj);

                if (value == null)
                {
                    result[propName] = "";
                }
                else if (IsSimple(value.GetType()))
                {
                    result[propName] = value.ToString();
                }
                else
                {
                    // Recursive flatten
                    var nested = FlattenObject(value, propName);
                    foreach (var kvp in nested)
                    {
                        result[kvp.Key] = kvp.Value;
                    }
                }
            }

            return result;
        }

        private static string ConvertObjToQueryStr(Dictionary<string, string> data)
        {
            return string.Join("&", data.Select(kvp =>
            {
                var value = kvp.Value;
                if (string.IsNullOrWhiteSpace(value) || value == "null" || value == "undefined")
                {
                    value = "";
                }
                return $"{kvp.Key}={value}";
            }));
        }

        private static bool IsSimple(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type.Equals(typeof(string)) ||
                type.Equals(typeof(decimal)) ||
                type.Equals(typeof(DateTime));
        }
    }
}
