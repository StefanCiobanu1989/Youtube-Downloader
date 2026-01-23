namespace YoutubeDownloader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            components = new System.ComponentModel.Container();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            button1 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            downloadVideoToolStripMenuItem = new ToolStripMenuItem();
            downloadMP3ToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(-3, 3);
            webView21.Name = "webView21";
            webView21.Size = new Size(1675, 644);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.DimGray;
            button1.ContextMenuStrip = contextMenuStrip1;
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.WhiteSmoke;
            button1.Location = new Point(1297, 9);
            button1.Name = "button1";
            button1.Size = new Size(110, 42);
            button1.TabIndex = 1;
            button1.Text = "Download";
            button1.UseVisualStyleBackColor = false;
            button1.Visible = false;
            button1.Click += button1_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.BackColor = SystemColors.ControlDarkDark;
            contextMenuStrip1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { downloadVideoToolStripMenuItem, downloadMP3ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.RenderMode = ToolStripRenderMode.Professional;
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.ShowItemToolTips = false;
            contextMenuStrip1.Size = new Size(177, 96);
            // 
            // downloadVideoToolStripMenuItem
            // 
            downloadVideoToolStripMenuItem.BackColor = Color.DimGray;
            downloadVideoToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            downloadVideoToolStripMenuItem.Name = "downloadVideoToolStripMenuItem";
            downloadVideoToolStripMenuItem.Padding = new Padding(10);
            downloadVideoToolStripMenuItem.Size = new Size(196, 44);
            downloadVideoToolStripMenuItem.Text = "Download Video";
            downloadVideoToolStripMenuItem.TextAlign = ContentAlignment.BottomCenter;
            downloadVideoToolStripMenuItem.TextDirection = ToolStripTextDirection.Horizontal;
            downloadVideoToolStripMenuItem.TextImageRelation = TextImageRelation.TextAboveImage;
            // 
            // downloadMP3ToolStripMenuItem
            // 
            downloadMP3ToolStripMenuItem.Checked = true;
            downloadMP3ToolStripMenuItem.CheckState = CheckState.Checked;
            downloadMP3ToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            downloadMP3ToolStripMenuItem.Name = "downloadMP3ToolStripMenuItem";
            downloadMP3ToolStripMenuItem.Size = new Size(176, 26);
            downloadMP3ToolStripMenuItem.Text = "Download MP3";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1675, 645);
            Controls.Add(button1);
            Controls.Add(webView21);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Youtube Downloader";
            Load += Form1_Load;
            ResizeEnd += Form1_ResizeEnd;
            Resize += Form1_Resize;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Button button1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem downloadVideoToolStripMenuItem;
        private ToolStripMenuItem downloadMP3ToolStripMenuItem;
    }
}
