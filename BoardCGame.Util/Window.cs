using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoardCGame
{
    public static class Window
    {
        public static bool ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 575,
                Height = 140,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            Button yes = new Button()
            {
                Text = "Sim", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.Yes
            
            };
            Button no = new Button()
            {
                Text = "Não", Left = 450, Width = 100, Top = 70, DialogResult = DialogResult.No
                    
            };
            yes.Click += (sender, e) => { prompt.Close(); };
            no.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(yes);
            prompt.Controls.Add(no);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = yes;
            prompt.CancelButton = no;

            return prompt.ShowDialog() == DialogResult.Yes;
        }
    }
}
