using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public class FilePanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        public FilePanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);
            var split = new SplitContainer {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = this.Width / 2,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true
            };
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, BackColor = Color.FromArgb(30, 30, 30) };
            var btnBack = new Button { Text = "⬅ Tilbake", AutoSize = true, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.White, Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(5) };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            // Funksjonsknapper med input der det trengs
            AddButton(layout, "📁 Åpne ofte brukt mappe", () => RunCmd("start \"\" \"C:\\IT-Verktøy\""));
            AddButtonWithInput(layout, "📄 Kopier fil", "Kilde", "Mål", (kilde, dest) => RunCmd($"copy \"{kilde}\" \"{dest}\""));
            AddButton(layout, "🧹 Slett midlertidige filer", () => RunCmd("del /q /s %temp%\\*.*"));
            AddButtonWithInput(layout, "🔍 Søk etter fil", "Filnavn", (filnavn) => RunCmd($"dir /s /b \"{filnavn}\""));
            AddButtonWithInput(layout, "📦 Pakk ut ZIP-fil", "ZIP-fil", "Målmappe", (zipfil, dest) => RunCmd($"powershell -Command \"Expand-Archive -Path '{zipfil}' -DestinationPath '{dest}'\""));
            AddButtonWithInput(layout, "📂 Opprett ny mappe", "Mappebane", (ny) => RunCmd($"mkdir \"{ny}\""));
            AddButtonWithInput(layout, "📦 Komprimer mappe til ZIP", "Mappe", "ZIP-fil", (mappe, zip) => RunCmd($"powershell -Command \"Compress-Archive -Path '{mappe}' -DestinationPath '{zip}'\""));
            outputBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            split.Panel1.Controls.Add(layout);
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
        }
        private void AddButton(FlowLayoutPanel panel, string text, Action action)
        {
            var btn = new Button
            {
                Text = text,
                AutoSize = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btn.Click += (s, e) => action();
            panel.Controls.Add(btn);
        }
        private void AddButtonWithInput(FlowLayoutPanel panel, string text, string prompt, Action<string> action)
        {
            var btn = new Button
            {
                Text = text,
                AutoSize = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btn.Click += (s, e) => {
                var input = Microsoft.VisualBasic.Interaction.InputBox($"{prompt}:", text, "");
                if (!string.IsNullOrWhiteSpace(input)) action(input);
            };
            panel.Controls.Add(btn);
        }
        private void AddButtonWithInput(FlowLayoutPanel panel, string text, string prompt1, string prompt2, Action<string, string> action)
        {
            var btn = new Button
            {
                Text = text,
                AutoSize = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btn.Click += (s, e) => {
                var input1 = Microsoft.VisualBasic.Interaction.InputBox($"{prompt1}:", text, "");
                var input2 = Microsoft.VisualBasic.Interaction.InputBox($"{prompt2}:", text, "");
                if (!string.IsNullOrWhiteSpace(input1) && !string.IsNullOrWhiteSpace(input2)) action(input1, input2);
            };
            panel.Controls.Add(btn);
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
    }
}
