using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public class OfficeCachePanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        public OfficeCachePanel()
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
            var layout = new FlowLayoutPanel {
                Dock = DockStyle.Top,
                AutoSize = false,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15, 20, 15, 20), // Mer padding rundt innholdet
                Height = 350,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            var btnBack = new Button { Text = "⬅ Tilbake", AutoSize = true, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.White, Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(5) };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            var infoBox = new TextBox {
                Text = "Tips: Hvis du har problemer med Office error 48v35, trykk på alle knappene i rekkefølge for best mulig sjanse til å fjerne feilen.",
                ReadOnly = true,
                Multiline = true,
                Dock = DockStyle.Top,
                Height = 48, // Litt høyere for å unngå kutting
                BackColor = Color.FromArgb(45, 45, 45), // Mørk bakgrunn
                ForeColor = Color.Yellow,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(6, 8, 6, 8) // Mer padding oppe og nede
            };
            layout.Controls.Add(infoBox);
            string[] btnTexts = {
                "🗑️ Slett Teams-cache",
                "🗑️ Slett OneNote-cache",
                "🗑️ Slett Office-cache (Word, PowerPoint, Excel, Outlook)",
                "🗑️ Slett Microsoft-legitimasjoner (Windows Credentials)"
            };
            string[] cmds = {
                // Teams-cache: Slett cache, temp og settings
                "powershell -NoProfile -Command \"Stop-Process -Name 'ms-teams' -Force -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force -ErrorAction Ignore $env:LOCALAPPDATA\\Packages\\MSTeams_8wekyb3d8bbwe\\LocalCache\\Microsoft\\MSTeams; Remove-Item -Recurse -Force -ErrorAction Ignore $env:APPDATA\\Microsoft\\Teams\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:LOCALAPPDATA\\Microsoft\\Teams\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\Teams\\*; Write-Host '✅ Teams-cache og temp slettet!';\"",
                // OneNote-cache: Slett cache og temp
                "powershell -NoProfile -Command \"Stop-Process -Name 'OneNote' -Force -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force -ErrorAction Ignore $env:LOCALAPPDATA\\Microsoft\\OneNote\\16.0\\cache; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\OneNote\\*; Write-Host '✅ OneNote-cache og temp slettet!';\"",
                // Office-cache: Slett cache, temp og settings for alle apper
                "powershell -NoProfile -Command \"Stop-Process -Name 'winword','excel','powerpnt','outlook' -Force -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force -ErrorAction Ignore $env:LOCALAPPDATA\\Microsoft\\Office\\16.0\\OfficeFileCache; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\Word\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\Excel\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\PowerPoint\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\\Outlook\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:APPDATA\\Microsoft\\Word\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:APPDATA\\Microsoft\\Excel\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:APPDATA\\Microsoft\\PowerPoint\\*; Remove-Item -Recurse -Force -ErrorAction Ignore $env:APPDATA\\Microsoft\\Outlook\\*; Write-Host '✅ Office-cache, temp og settings slettet!';\"",
                // Microsoft-legitimasjoner: Slett legitimasjoner som inneholder 'Microsoft'
                "powershell -NoProfile -Command \"cmdkey /list | Select-String 'Microsoft' | ForEach-Object { $cred = $_.Line -replace '.*Target: ', ''; cmdkey /delete:$cred }; Write-Host '✅ Microsoft-legitimasjoner slettet!'; Restart-Computer -Force\""
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
                    Margin = new Padding(0, 0, 0, 15) // Mer mellomrom under hver knapp
                };
                int idx = i;
                btn.Click += (s, e) => RunCmd(cmds[idx]);
                layout.Controls.Add(btn);
            }
            Color lilla = Color.FromArgb(120, 60, 200);
            foreach (var btn in layout.Controls.OfType<Button>())
            {
                btn.BackColor = lilla;
                btn.ForeColor = Color.White;
            }
            int maxBtnWidth = 0;
            foreach (Control c in layout.Controls)
                if (c.Width > maxBtnWidth) maxBtnWidth = c.Width;
            int minMenuWidth = maxBtnWidth + 40;
            split.SplitterDistance = Math.Max((int)(this.Width * 0.35), minMenuWidth);
            this.Resize += (s, e) => {
                int newMaxBtnWidth = 0;
                foreach (Control c in layout.Controls)
                    if (c.Width > newMaxBtnWidth) newMaxBtnWidth = c.Width;
                int newMinMenuWidth = newMaxBtnWidth + 40;
                split.SplitterDistance = Math.Max((int)(this.Width * 0.35), newMinMenuWidth);
            };
            split.Panel1.Controls.Add(layout);
            outputBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
        }
        private async void RunCmd(string cmd)
        {
            outputBox.Clear();
            string action = "";
            Color lilla = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla;
            if (cmd.Contains("Teams")) action = "Sletter Teams-cache...";
            else if (cmd.Contains("OneNote")) action = "Sletter OneNote-cache...";
            else if (cmd.Contains("OfficeFileCache")) action = "Sletter Office-cache...";
            else if (cmd.Contains("cmdkey")) action = "Sletter Microsoft-legitimasjoner og starter om...";
            else action = "Utfører handling...";
            outputBox.AppendText($"▶ {action}\r\n");
            try
            {
                var proc = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "cmd.exe",
                        Arguments = $"/c {cmd}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                string stdOut = await proc.StandardOutput.ReadToEndAsync();
                string stdErr = await proc.StandardError.ReadToEndAsync();
                proc.WaitForExit();
                if (!string.IsNullOrWhiteSpace(stdOut))
                    outputBox.AppendText($"{stdOut.Trim()}\r\n");
                if (!string.IsNullOrWhiteSpace(stdErr))
                    outputBox.AppendText($"❌ Feil: {stdErr.Trim()}\r\n");
                if (proc.ExitCode == 0)
                    outputBox.AppendText($"✅ Ferdig: {action.Replace("Sletter", "Slettet")}\r\n");
                else
                    outputBox.AppendText($"❌ Noe gikk galt under {action.ToLower()}\r\n");
            }
            catch (Exception ex)
            {
                outputBox.AppendText($"❌ Unntak: {ex.Message}\r\n");
            }
        }
    }
}
