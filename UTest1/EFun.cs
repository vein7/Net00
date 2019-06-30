using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTest1 {
    static class EFun {

        public static DateTime ToUnixTime(this int num) {
            //621355968000000000 - 621355968000000000
            return DateTimeOffset.FromUnixTimeSeconds(num).LocalDateTime;
        }
    }
}
