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
            Color lilla = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla;
            foreach (var btn in layout.Controls.OfType<Button>())
            {
                btn.BackColor = lilla;
                btn.ForeColor = Color.White;
            }
        }
        private async void RunCmd(string cmd)
        {
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
