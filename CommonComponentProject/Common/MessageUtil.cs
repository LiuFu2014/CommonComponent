using System;
using System.Windows.Forms;


namespace Common
{
    public class MessageUtil
    {
        public static DialogResult ShowError(string message)
        {
            return MessageBox.Show(message, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static DialogResult ShowTips(string message)
        {
            return MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public static DialogResult ShowWarning(string message)
        {
            return MessageBox.Show(message, "警告信息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static DialogResult ShowYesNoAndError(string message)
        {
            return MessageBox.Show(message, "错误信息", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
        }

        public static DialogResult ShowYesNoAndTips(string message)
        {
            return MessageBox.Show(message, "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
        }

        public static DialogResult ShowYesNoAndWarning(string message)
        {
            return MessageBox.Show(message, "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }

        public static DialogResult ShowYesNoCancelAndTips(string message)
        {
            return MessageBox.Show(message, "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
        }
        //Testtest
        //from remoting TestBanch
        //jdlfjalksdjf zaici ceshi 的
    }
}


