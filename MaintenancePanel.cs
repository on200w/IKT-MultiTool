using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public class MaintenancePanel : UserControl
    {
        private TextBox outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        public MaintenancePanel()
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
            AddButton(layout, "🔄 Fornye IP-adresse og teste nettverk", () => RunCmd("ipconfig /release & ipconfig /renew & ping 1.1.1.1 -n 3 & ping google.com -n 3"));
            AddButton(layout, "📡 Sjekk Windows Update-status", () => RunCmd("sc query wuauserv"));
            AddButton(layout, "🛡️ Kjør SFC (System File Checker)", () => RunSfc());
            AddButton(layout, "🛠️ Kjør DISM reparasjon", () => RunCmd("DISM /Online /Cleanup-Image /RestoreHealth"));
            AddButton(layout, "🧪 Test disk for feil", () => RunCmd("chkdsk C:"));
            AddButton(layout, "🧹 Rydd opp i Windows Update-cache", () => RunCmd("net stop wuauserv & del /s /q %windir%\\SoftwareDistribution\\*.* & net start wuauserv"));
            AddButton(layout, "🌐 Test internettforbindelse", () => RunCmd("ping 1.1.1.1 -n 5 & ping google.com -n 5"));
            AddButton(layout, "📑 Åpne hendelseslogg", () => RunCmd("eventvwr"));
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
        private void SetButtonsEnabled(bool enabled)
        {
            foreach (Control c in this.Controls)
            {
                if (c is SplitContainer split)
                {
                    foreach (Control panelControl in split.Panel1.Controls)
                    {
                        if (panelControl is FlowLayoutPanel flp)
                        {
                            foreach (Control btn in flp.Controls)
                            {
                                if (btn is Button b)
                                    b.Enabled = enabled;
                            }
                        }
                    }
                }
            }
        }
        private async void RunSfc()
        {
            SetButtonsEnabled(false);
            outputBox.Text = "SFC kjører, dette kan ta flere minutter...\r\n";
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo("cmd.exe", "/c sfc /scannow")
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
                        outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText("\r\nSFC er ferdig.")));
                    }
                    else
                    {
                        outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Kunne ikke starte SFC-prosess."));
                    }
                }
                catch (Exception ex)
                {
                    outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Feil: " + ex.Message));
                }
            });
            SetButtonsEnabled(true);
        }
        private async void RunCmd(string cmd)
        {
            SetButtonsEnabled(false);
            await Task.Run(() =>
            {
                try
                {
                    outputBox.Invoke((MethodInvoker)(() => outputBox.Text = "Kjører kommando...\r\n"));
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
            SetButtonsEnabled(true);
        }
    }
}
