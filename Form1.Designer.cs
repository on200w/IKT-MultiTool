namespace IKTMultiTool;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Panel mainPanel;
    private System.Windows.Forms.Button btnNettverk;
    private System.Windows.Forms.Button btnSystem;
    private System.Windows.Forms.Button btnFeilsoking;
    private System.Windows.Forms.Button btnCache;
    private System.Windows.Forms.Button btnDrivers;
    private System.Windows.Forms.Button btnAvslutt;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(1400, 800); // Litt høyere vindu for mer plass nederst
        this.Text = "IKT MultiTool";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(30, 30, 30) };
        this.Controls.Add(mainPanel);
        // Beregn midtstilling
        int panelWidth = this.ClientSize.Width;
        int startY = 50;
        int spacing = 15;
        int btnWidth = 0;
        int btnHeight = 0;

        btnNettverk = new Button { Text = "🌐 Nettverk", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.DeepSkyBlue, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };
        btnSystem = new Button { Text = "💻 System", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.LimeGreen, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };
        btnFeilsoking = new Button { Text = "🔍 Feilsøking og vedlikehold", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.Violet, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };
        btnCache = new Button { Text = "🧹 Rydd cache for Office-apper (48v35 error)", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.Pink, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };
        btnDrivers = new Button { Text = "💾 Drivere", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.Orange, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };
        btnAvslutt = new Button { Text = "❌ Avslutt", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.FromArgb(60,0,0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Emoji", 15, FontStyle.Bold), Padding = new Padding(14, 8, 14, 8) };

        Button[] buttons = { btnNettverk, btnSystem, btnFeilsoking, btnCache, btnDrivers };
        Color lilla = Color.FromArgb(120, 60, 200); // Lilla farge
        Color avsluttFarge = Color.FromArgb(45,45,45); // Original avslutt-knappfarge
        foreach (var btn in buttons)
        {
            btn.BackColor = lilla;
            btn.ForeColor = Color.White;
        }
        btnAvslutt.BackColor = avsluttFarge;
        btnAvslutt.ForeColor = Color.Gold;

        for (int i = 0; i < buttons.Length; i++)
        {
            btnWidth = buttons[i].PreferredSize.Width;
            btnHeight = buttons[i].PreferredSize.Height;
            buttons[i].Location = new Point((panelWidth - btnWidth) / 2, startY + i * (btnHeight + spacing));
            mainPanel.Controls.Add(buttons[i]);
        }
        btnNettverk.Click += (s, e) => ShowCategory("Nettverk");
        btnSystem.Click += (s, e) => ShowCategory("System");
        btnFeilsoking.Click += (s, e) => ShowCategory("Feilsoking");
        btnCache.Click += (s, e) => ShowCategory("Cache");
        btnDrivers.Click += (s, e) => ShowCategory("Drivere");
        btnAvslutt.Click += (s, e) => this.Close();
    }

    #endregion
}
