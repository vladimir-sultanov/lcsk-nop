using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.LiveChat.Models
{
    public class CaptchaRequest
    {
        public string Challenge { get; set; }
        public string Response { get; set; }
    }
}
