using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaferPolishingSystem.Define
{
    class UserMsgBox
    {
        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            //Application.ProductName
            return MessageBox.Show(text, "POLISHING 1.8" , buttons, icon, defaultButton);
        }
        //--------------------------------------------------------------------------
        public static void Show(string text)
        {
            Show(text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //--------------------------------------------------------------------------
        public static void Error(string text)
        {
            Show(text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        //--------------------------------------------------------------------------
        public static void Error(string fmt, params object[] args)
        {
            Error(string.Format(fmt, args));
        }
        //--------------------------------------------------------------------------
        public static void Warning(string text)
        {
            Show(text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //--------------------------------------------------------------------------
        public static bool Confirm(string text)
        {
            return Show(text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }
        //--------------------------------------------------------------------------
        public static bool Confirm(string fmt, params object[] args)
        {
            return Confirm(string.Format(fmt, args));
        }
    }
}
