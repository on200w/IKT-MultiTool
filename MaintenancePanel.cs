using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
            // Logger fjernet for panelvalg
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);
            
            split = new SplitContainer {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true, // Fast splitter
                SplitterWidth = 4,
                SplitterDistance = (int)(this.Width * 0.35) // 35% til knapper, 65% til kommandolinje
            };
            
            var layout = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(0, 0, 12, 28), // Litt bunn-padding; fast bunnluft håndteres separat
                AutoScrollMargin = new Size(0, 40)
            };
            
            var btnBack = new Button { 
                Text = "⬅ Tilbake", 
                Size = new Size(90, 80), // Fast størrelse
                BackColor = Color.FromArgb(45,45,45), 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), 
                FlatStyle = FlatStyle.Flat, 
                Margin = new Padding(5),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8)
            };
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
            // Fast bunnluft via tabellcontainer
            var leftTable = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ColumnCount = 1,
                RowCount = 2
            };
            leftTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leftTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            leftTable.Controls.Add(layout, 0, 0);
            leftTable.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30,30,30) }, 0, 1);
            var leftContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30,30,30), Padding = new Padding(0,0,0,8) };
            leftContainer.Controls.Add(leftTable);
            split.Panel1.Controls.Add(leftContainer);
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
            
            Color lilla = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla;
            foreach (var btn in layout.Controls.OfType<Button>())
            {
                btn.BackColor = lilla;
                btn.ForeColor = Color.White;
                btn.Height = 60; // konsistent høyde
                btn.Font = new Font("Segoe UI Emoji", 10, FontStyle.Regular);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Margin = new Padding(0, 6, 8, 6);
            }
            // Dynamisk bredde så knappene fyller meny-området
            layout.SizeChanged += (s, e) => {
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
            };

            // Hold splitter i ~35/65 ved resize
            this.SizeChanged += (s, e) => {
                try
                {
                    var width = this.ClientSize.Width;
                    split.SplitterDistance = Math.Max(120, (int)(width * 0.35));
                }
                catch { }
            };
        }
        private void AddButton(FlowLayoutPanel panel, string text, Action action)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(90, 80), // startstørrelse, bredden overstyres
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8)
            };
            btn.Click += (s, e) => {
                Logger.Log($"MaintenancePanel: Klikket på knapp '{text}'");
                action();
            };
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
            Logger.Log("MaintenancePanel: Kjører SFC /scannow");
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
            Logger.Log($"MaintenancePanel: Kjører kommando: {cmd}");
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
