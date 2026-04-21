using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.Helpers
{
    public static class EnumHelper
    {
        public static bool TryParseEnum<T>(string value, out T result) where T : struct
        {
            return Enum.TryParse(value, true, out result);
        }

        public static List<string> GetSpecializationsList(Specializations specs)
        {
            return Enum.GetValues(typeof(Specializations))
                .Cast<Specializations>()
                .Where(s => s != Specializations.None && specs.HasFlag(s))
                .Select(s => s.ToString())
                .ToList();
        }
    }
}
