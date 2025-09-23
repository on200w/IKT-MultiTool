using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public partial class NetworkPanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        public NetworkPanel()
        {
            // Logger fjernet for panelvalg
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);
            
            split = new SplitContainer {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true,
                SplitterDistance = (int)(this.Width * 0.35), // 35% til knapper, 65% til kommandolinje
                SplitterWidth = 4
            };
            // Container som gir fast luft nederst uavhengig av scrolling
            var leftContainer = new Panel {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(0) // vi håndterer bunnluft via en egen bunnrad
            };
            
            var layout = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(0, 0, 12, 28), // Litt bunn-padding for synlig luft når man scroller helt ned
                AutoScrollMargin = new Size(0, 48)
            };
            
            var btnBack = new Button { 
                Text = "⬅ Tilbake", 
                BackColor = Color.FromArgb(45,45,45), 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), 
                FlatStyle = FlatStyle.Flat, 
                Margin = new Padding(5),
                Size = new Size(350, 40), // Fast størrelse
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            string[] btnTexts = {
                "📡 Vis IP-konfigurasjon", "📶 Ping Google DNS", "🛰️ Traceroute til google.com", "🧹 Tøm DNS-cache",
                "🔍 Vis aktive nettverkstilkoblinger", "🌍 Vis offentlig IP-adresse", "📜 Liste lagrede Wi-Fi-profiler",
                "💳 Vis MAC-adresse (med type)", "🚀 Test internett-hastighet / last ned okla cli"
            };
            Action[] actions = {
                () => RunCmd("ipconfig /all"),
                () => RunCmd("ping 8.8.8.8"),
                () => RunCmd("tracert google.com"),
                () => RunCmd("ipconfig /flushdns"),
                () => RunCmd("netstat -ano"),
                () => RunCmd("nslookup myip.opendns.com resolver1.opendns.com"),
                () => RunCmd("netsh wlan show profiles"),
                () => RunMacCmd(),
                () => RunCmd("speedtest")
            };
            Button? lastButton = null;
            for (int i = 0; i < btnTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = btnTexts[i],
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5),
                    Size = new Size(120, 100), // Større knapper
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 8, 10, 8)
                };
                int idx = i;
                btn.Click += (s, e) => {
                    Logger.Log($"NetworkPanel: Klikket på knapp '{btnTexts[idx]}'");
                    actions[idx]();
                };
                layout.Controls.Add(btn);
                lastButton = btn;
            }
            
            // Trigger en resize for å sette riktig splitter-posisjon
            this.OnResize(EventArgs.Empty);
            
            outputBox = new TextBox { 
                Multiline = true, 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                BackColor = Color.Black, 
                ForeColor = Color.Lime, 
                Font = new Font("Consolas", 9), 
                ScrollBars = ScrollBars.Vertical, 
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true
            };
            // Legg layout i en tabell med fast bunnrad som alltid er synlig
            var leftTable = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ColumnCount = 1,
                RowCount = 2
            };
            leftTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leftTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F)); // alltid synlig luft nederst
            leftTable.Controls.Add(layout, 0, 0);
            var bottomGap = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };
            leftTable.Controls.Add(bottomGap, 0, 1);
            leftContainer.Controls.Add(leftTable);
            split.Panel1.Controls.Add(leftContainer);
            // Liten ekstra padding helt nederst i Panel1
            split.Panel1.Padding = new Padding(0, 0, 0, 8);
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
            
            Color lilla = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla;
            foreach (var btn in layout.Controls.OfType<Button>())
            {
                btn.BackColor = lilla;
                btn.ForeColor = Color.White;
                btn.Height = 60;
                btn.Font = new Font("Segoe UI Emoji", 10, FontStyle.Regular);
                btn.TextAlign = ContentAlignment.MiddleLeft; // tekst mot venstre
                btn.Margin = new Padding(0, 6, 8, 6); // minimal venstremarg
            }
            // Gi siste knapp ekstra bunnmarg for synlig luft
            if (lastButton != null) lastButton.Margin = new Padding(0, 6, 8, 24);
            // Sett initial full bredde på knapper umiddelbart
            {
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
            }
            // Dynamisk bredde for knapper
            layout.SizeChanged += (s, e) => {
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
            };
            // Ingen ekstra scrollbart spacer; bunnluft ligger i fast bunnrad som alltid er synlig

            // Hold splitter i ~35/65 ved resize
            this.SizeChanged += (s, e) => {
                try
                {
                    var width = this.ClientSize.Width;
                    split.SplitterDistance = Math.Max(120, (int)(width * 0.35));
                }
                catch { /* ignorer midlertidige layout-tilstander */ }
            };
        }
        private async void RunCmd(string cmd)
        {
            Logger.Log($"NetworkPanel: Kjører kommando: {cmd}");
            outputBox.Text = "Kjører kommando...\r\n";
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                try
                {
                    var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        while (!proc.StandardOutput.EndOfStream)
                        {
                            string? line = proc.StandardOutput.ReadLine();
                            if (line != null)
                                outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText(line + "\r\n")));
                        }
                        string error = proc.StandardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(error))
                            outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText(error)));
                        proc.WaitForExit();
                    }
                    else
                    {
                        outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Kunne ikke starte prosess."));
                    }
                }
                catch (Exception ex)
                {
                    outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Feil: " + ex.Message));
                }
            });
        }
        private async void RunMacCmd()
        {
            Logger.Log("NetworkPanel: Kjører MAC-adresse-kommando");
            outputBox.Text = "Henter MAC-adresser...\r\n";
            await Task.Run(() =>
            {
                var psCmd = "Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | ForEach-Object { $type = if ($_.InterfaceDescription -match 'Wi-Fi|Wireless') { 'Wi-Fi' } elseif ($_.InterfaceDescription -match 'Ethernet') { 'Ethernet' } else { 'Annet' }; Write-Output \"Type: $type`r`nNavn: $($_.Name)`r`nMAC: $($_.MacAddress)`r`n\" }";
                var psi = new ProcessStartInfo("powershell", $"-NoProfile -Command \"{psCmd}\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                try
                {
                    var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        while (!proc.StandardOutput.EndOfStream)
                        {
                            string? line = proc.StandardOutput.ReadLine();
                            if (line != null)
                                outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText(line + "\r\n")));
                        }
                        string error = proc.StandardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(error))
                            outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText(error)));
                        proc.WaitForExit();
                    }
                    else
                    {
                        outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Kunne ikke starte PowerShell-prosess."));
                    }
                }
                catch (Exception ex)
                {
                    outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Feil: " + ex.Message));
                }
            });
        }
    }
}
