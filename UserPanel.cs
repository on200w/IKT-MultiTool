using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public partial class UserPanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        public UserPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);
            split = new SplitContainer {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true
            };
            this.Resize += (s, e) => split.SplitterDistance = (int)(this.Width * 0.35);
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, BackColor = Color.FromArgb(30, 30, 30) };
            var btnBack = new Button { Text = "⬅ Tilbake", AutoSize = true, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.White, Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(5) };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            string[] btnTexts = {
                "📜 Liste brukere", "➕ Legg til bruker", "➖ Slett bruker", "🔑 Endre passord",
                "👥 Sjekk påloggede brukere", "🛡️ Endre brukertype", "🔒 Lås bruker", "🔓 Aktiver bruker"
            };
            Action[] actions = {
                () => RunCmd("net user"),
                () => RunCmdInput("net user {0} /add", "Brukernavn"),
                () => RunCmdInput("net user {0} /delete", "Brukernavn"),
                () => RunCmdInput("net user {0} *", "Brukernavn"),
                () => RunCmd("query user"),
                () => RunCmdInput("net localgroup {1} {0} /add", "Brukernavn", "Type (Administrators/Users)"),
                () => RunCmdInput("net user {0} /active:no", "Brukernavn"),
                () => RunCmdInput("net user {0} /active:yes", "Brukernavn")
            };
            for (int i = 0; i < btnTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = btnTexts[i],
                    AutoSize = true,
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5)
                };
                int idx = i;
                btn.Click += (s, e) => actions[idx]();
                layout.Controls.Add(btn);
            }
            int maxBtnWidth = 0;
            foreach (Control c in layout.Controls)
                if (c.Width > maxBtnWidth) maxBtnWidth = c.Width;
            int minMenuWidth = maxBtnWidth + 40;
            if (split.SplitterDistance < minMenuWidth)
                split.SplitterDistance = minMenuWidth;
            outputBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            split.Panel1.Controls.Add(layout);
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
        }
        private async void RunCmd(string cmd)
        {
            try
            {
                outputBox.Text = "Kjører kommando...\r\n";
                var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        string? line = await proc.StandardOutput.ReadLineAsync();
                        if (line != null) outputBox.AppendText(line + "\r\n");
                    }
                    string error = await proc.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(error))
                        outputBox.AppendText(error);
                    proc.WaitForExit();
                }
                else
                {
                    outputBox.Text = "Kunne ikke starte prosess.";
                }
            }
            catch (Exception ex)
            {
                outputBox.Text = "Feil: " + ex.Message;
            }
        }
        private void RunCmdInput(string cmdFormat, params string[] prompts)
        {
            string[] values = new string[prompts.Length];
            for (int i = 0; i < prompts.Length; i++)
            {
                values[i] = Microsoft.VisualBasic.Interaction.InputBox(prompts[i], "Input", "");
                if (string.IsNullOrWhiteSpace(values[i])) return;
            }
            RunCmd(string.Format(cmdFormat, values));
        }
    }
}
