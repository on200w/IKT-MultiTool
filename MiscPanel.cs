using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public class MiscPanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        public MiscPanel()
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
            AddButton(layout, "🖱️ Åpne Enhetsbehandling", () => RunCmd("start devmgmt.msc"));
            AddButton(layout, "📊 Åpne Oppgavebehandling", () => RunCmd("start taskmgr"));
            AddButton(layout, "⚙️ Åpne Kontrollpanel", () => RunCmd("start control"));
            AddButton(layout, "⏰ Vise dato og klokkeslett", () => RunCmd("echo Dato: %date% & echo Tid: %time%"));
            AddButton(layout, "🗑️ Tømme papirkurv", () => RunCmd("powershell -NoProfile -Command \"$drives = (Get-PSDrive -PSProvider FileSystem).Name; foreach($dl in $drives){ try { Clear-RecycleBin -DriveLetter $dl -Force -ErrorAction Stop; Write-Host ('Tømte papirkurv på {0}:' -f $dl) -ForegroundColor Green } catch { try { Remove-Item -LiteralPath ($dl + ':\\$Recycle.Bin') -Recurse -Force -ErrorAction Stop; Write-Host ('Fjernet $Recycle.Bin på {0}:' -f $dl) -ForegroundColor Yellow } catch { Write-Host ('Feilet på {0}:' -f $dl) -ForegroundColor Red } } }; Write-Host ''\""));
            AddButton(layout, "📸 Ta skjermbilde (Print Screen)", () => RunCmd("echo Trykk Print Screen-tasten for å ta skjermbilde"));
            AddButton(layout, "💻 Åpne PowerShell som admin", () => RunCmd("powershell -Command \"Start-Process PowerShell -Verb RunAs\""));
            AddButtonWithInput(layout, "⏳ Slå av PC etter X sekunder", "Sekunder til avstenging", (sek) => RunCmd($"shutdown /s /t {sek}"));
            outputBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            int maxBtnWidth = 0;
            foreach (Control c in layout.Controls)
                if (c.Width > maxBtnWidth) maxBtnWidth = c.Width;
            int minMenuWidth = maxBtnWidth + 40;
            if (split.SplitterDistance < minMenuWidth)
                split.SplitterDistance = minMenuWidth;
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
