using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {

        // Import the SendMessageTimeout function from the user32.dll Windows library.
        // DllImport allows calling unmanaged code (native Windows API) from managed C# code.
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]

        // Declare the method as private and static because it is only used within this class
        // and does not require an instance of the class to call.
        private static extern IntPtr SendMessageTimeout(

            // Handle to the window that will receive the message.
            IntPtr hWnd,

            // The message to send (WM_... constant, e.g., WM_SETTEXT).
            uint Msg,

            // Additional message-specific information (first parameter, often a pointer or handle).
            IntPtr wParam,

            // Additional message-specific information (second parameter, often a string or pointer).
            string lParam,

            // Flags that control the behavior of SendMessageTimeout (e.g., SMTO_ABORTIFHUNG).
            uint fuFlags,

            // Timeout value in milliseconds; if the message is not processed in this time, it returns.
            uint uTimeout,

            // Out parameter that receives the result of the message processing.
            out IntPtr lpdwResult
        );


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Build the full path to the Deno executable inside the "libs" folder of your application's startup directory
            string denoPath = Path.Combine(Application.StartupPath, "libs", "deno.exe");

            // Check if the Deno executable does NOT exist at that path
            if (!File.Exists(denoPath))
            {
                // If it doesn't exist, extract Deno from the ZIP file located at "libs/deno.zip"
                UnzipDeno(Path.Combine(Application.StartupPath, "libs", "deno.zip"));

                // Restart the application so that it can use Deno immediately after extraction
                Application.Restart();
            }

            // Check if the "libs" folder exists in the application's startup directory.
            if (!IsPathPresent(Path.Combine(Application.StartupPath, "libs")))
            {
                // Show a message box notifying the user that the path was not found and a new path will be set.
                MessageBox.Show("Path not found!" + Environment.NewLine + "Setting new path...");

                try
                {
                    // Combine the startup path with "libs" to get the full folder path.
                    string folder = Path.Combine(Application.StartupPath, "libs");

                    // Call a method to add this folder to the system PATH environment variable.
                    AddToSystemPath(folder);

                    // Notify the user that the folder has been successfully added to the system PATH.
                    MessageBox.Show("Folder added to system PATH.");
                }
                // Catch an exception if the program does not have permission to modify the system PATH.
                catch (UnauthorizedAccessException)
                {
                    // Notify the user that administrator privileges are required to modify the system PATH.
                    MessageBox.Show("Run as administrator to modify system PATH.");
                }
                // Catch any other exceptions that might occur and display the error message.
                catch (Exception ex)
                {
                    // Show a message box with the error details.
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            // If the "libs" folder already exists, set the webView21 control to display YouTube.
            else
            {
                webView21.Source = new Uri("https://www.youtube.com");
            }

            // Subscribe to the SourceChanged event of webView21 so that WebView21_SourceChanged
            // method will be called whenever the source (URL) of the WebView changes.
            webView21.SourceChanged += WebView21_SourceChanged;

        }

        private void WebView21_SourceChanged(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            // Check if the WebView currently has a valid URL loaded.
            if (webView21.Source != null)
            {
                // Check if the URL contains "watch?v=", which indicates a YouTube video page.
                if (webView21.Source.ToString().Contains("watch?v="))
                {
                    // Populate a menu or UI related to YouTube video downloading.
                    PopulateYtDlpMenu();

                    // Update the window title to show that a YouTube video is loaded,
                    // appending the current URL for reference.
                    this.Text = "Youtube Downloader" + " -> " + webView21.Source.ToString();
                }
                else
                {
                    // If the URL is not a YouTube video page, hide the download button (button1).
                    button1.Visible = false;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if the context menu is currently not visible.
            if (!contextMenuStrip1.Visible)
                // If it is not visible, show the context menu right below button1.
                // The coordinates (0, button1.Height) position it at the left edge of the button,
                // just below the bottom of the button.
                contextMenuStrip1.Show(button1, 0, button1.Height);
            else
                // If the context menu is already visible, close (hide) it.
                contextMenuStrip1.Close();

        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            // Check if the current width of the form/window is less than 1447 pixels.
            if (Width < 1447)
            {
                // If it is, set the width to 1447 pixels to enforce a minimum width.
                Width = 1447;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Check if the current width of the window/form is less than 1447 pixels.
            if (Width < 1447)
            {
                // If the width is too small, force it to be 1447 pixels.
                Width = 1447;
            }

        }





        private string[] RunCommand(string exePath, string arguments)
        {
            // Create a new ProcessStartInfo object, which contains information about the process to start.
            var psi = new ProcessStartInfo
            {
                // The path to the executable to run.
                FileName = exePath,

                // The command-line arguments to pass to the executable.
                Arguments = arguments,

                // Redirect the standard output so we can capture it in code.
                RedirectStandardOutput = true,

                // Must be false to redirect output; prevents using the shell to start the process.
                UseShellExecute = false,

                // Do not create a new window for the process (run it silently in the background).
                CreateNoWindow = true
            };

            // Start the process using the ProcessStartInfo configuration.
            using (var process = Process.Start(psi))
            {
                // Read all output produced by the process until it exits.
                string output = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit before continuing.
                process.WaitForExit();

                // Split the captured output into lines (handling both Windows and Unix line endings)
                // and remove any empty lines, returning the result as a string array.
                return output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

        }

        private (string tag, string format, string resolution, string size)[] ParseYtDlpOutput(string[] lines)
        {
            // Create a list to store tuples containing tag, format, resolution, and size for each video.
            var result = new List<(string tag, string format, string resolution, string size)>();

            // Iterate through each line in the input lines (typically output from a video listing).
            foreach (var line in lines)
            {
                // Skip the line if it is empty or contains only whitespace.
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Skip the line if it does not contain "mp4" or "webm" (case-insensitive check for video formats).
                if (!Regex.IsMatch(line, @"\b(mp4|webm)\b", RegexOptions.IgnoreCase)) continue;

                // Extract the first "word" in the line, which is assumed to be the format tag.
                string tagValue = line.Split(' ', 2)[0];

                // Extract the video format (mp4 or webm) from the line using a regex.
                string format = Regex.Match(line, @"\b(mp4|webm)\b", RegexOptions.IgnoreCase).Value;

                // Extract the video resolution (e.g., 720p, 1080p) from the line using a regex.
                string resolution = Regex.Match(line, @"\b\d{2,4}p\b").Value;

                // Extract the video file size (e.g., 15.2MiB, 1.4GB) using a regex with a capturing group.
                string size = Regex.Match(line, @"\|\s+([\d\.]+[KMGT]?i?B)").Groups[1].Value;

                // Only add the entry to the result if all extracted values are non-empty.
                if (!string.IsNullOrEmpty(format) && !string.IsNullOrEmpty(resolution) && !string.IsNullOrEmpty(size))
                {
                    result.Add((tagValue, format, resolution, size));
                }
            }

            // Convert the list of tuples to an array and return it.
            return result.ToArray();

        }

        private void PopulateYtDlpMenu()
        {
            // If the WebView2 has no URL loaded, exit the method early.
            if (webView21.Source == null) return;

            // Build the full path to the yt-dlp executable inside the "libs" folder.
            string ytDlpPath = Path.Combine(Application.StartupPath, "libs", "yt-dlp.exe");

            // Get the current URL from the WebView as a string.
            string videoUrl = webView21.Source.ToString();

            // Build the command-line arguments for yt-dlp:
            // --cookies-from-browser firefox  → use Firefox browser cookies
            // --no-playlist                  → only download single video
            // -F                             → list available formats
            // videoUrl                        → the video URL
            string args = " --cookies-from-browser firefox --no-playlist -F \"" + videoUrl + "\"";

            // Run yt-dlp with the constructed arguments and capture its output lines.
            string[] lines = RunCommand(ytDlpPath, args);

            // Parse the yt-dlp output into a structured format (tag, format, resolution, size).
            var parsedLines = ParseYtDlpOutput(lines);

            // Sort the parsed lines so that mp4 formats appear first, then webm.
            var sorted = parsedLines.OrderBy(x => x.format.ToLower() == "mp4" ? 0 : 1).ToList();

            // Clear any existing items in the context menu.
            contextMenuStrip1.Items.Clear();

            // --- Download Video Section ---
            // Create a new menu item for downloading videos.
            var downloadVideo = new ToolStripMenuItem("Download Video");
            downloadVideo.ForeColor = Color.WhiteSmoke;  // Set text color
            downloadVideo.BackColor = Color.DimGray;     // Set background color

            // Add each available video format as a submenu under "Download Video".
            foreach (var item in sorted)
            {
                // Create a menu item with format, resolution, and size.
                var formatItem = new ToolStripMenuItem($"{item.format} | {item.resolution} | {item.size}");
                formatItem.Tag = item.tag;                  // Store the format tag for later use
                formatItem.ForeColor = Color.WhiteSmoke;
                formatItem.BackColor = Color.DimGray;

                // Handle the click event: when user clicks this format, download it.
                formatItem.Click += (s, e) =>
                {
                    // Extract the extension (mp4 or webm) from the menu text.
                    string extension = formatItem.Text.Split('|')[0].Trim();

                    // Call method to download the video with yt-dlp using the selected format.
                    DownloadYtDlpFormat(ytDlpPath, formatItem.Tag.ToString(), videoUrl, extension);
                };

                // Add the format menu item to the dropdown of "Download Video".
                downloadVideo.DropDownItems.Add(formatItem);
            }

            // Add the "Download Video" menu to the context menu strip.
            contextMenuStrip1.Items.Add(downloadVideo);

            // --- Download Audio Section ---
            // Create a new menu item for downloading audio only.
            var downloadAudio = new ToolStripMenuItem("Download Audio");
            downloadAudio.ForeColor = Color.WhiteSmoke;

            // Handle click event: download the video as MP3.
            downloadAudio.Click += (s, e) =>
            {
                DownloadAsMp3(ytDlpPath, videoUrl);
            };

            // Add the "Download Audio" menu to the context menu strip.
            contextMenuStrip1.Items.Add(downloadAudio);

            // Make the main download button visible now that the menu is ready.
            button1.Visible = true;


        }

        private async void DownloadYtDlpFormat(string exePath, string formatCode, string videoUrl, string formatExtension)
        {
            // Step 1: Get the video title from the WebView2's current page using JavaScript.
            string pageTitle = await webView21.CoreWebView2.ExecuteScriptAsync("document.title;");

            // The result returned by ExecuteScriptAsync is a JSON string, so remove the surrounding quotes.
            pageTitle = pageTitle.Trim('"');

            // Store the page title as a fallback for the video title.
            string videoTitle = pageTitle;

            // Step 1b: Sanitize the filename to remove invalid characters for Windows file system.
            videoTitle = SanitizeFileName(videoTitle);

            // Step 2: Create a SaveFileDialog to ask the user where to save the video file.
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Where to save the file?";
            saveFile.FileName = $"{videoTitle}.{formatExtension}";  // Default filename with sanitized title and extension
            saveFile.Filter = $"{formatExtension.ToUpper()} files|*.{formatExtension}|All files|*.*"; // File type filter

            // Show the SaveFileDialog and check if the user clicked "OK".
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                // Step 3: Set up ProcessStartInfo to run yt-dlp with the chosen options.
                var psi = new ProcessStartInfo
                {
                    FileName = exePath,  // Path to yt-dlp executable

                    // Arguments for yt-dlp:
                    // --cookies-from-browser firefox → use cookies from Firefox
                    // --no-playlist                  → download only this video
                    // -f {formatCode}+bestaudio      → select the chosen video format + best audio
                    // -o "filename"                  → output file path
                    Arguments = $"--cookies-from-browser firefox --no-playlist -f {formatCode}+bestaudio \"{videoUrl}\" -o \"{saveFile.FileName}\"",

                    UseShellExecute = false,      // Do not use the system shell
                    RedirectStandardOutput = false, // Not capturing console output
                    RedirectStandardError = false,  // Not capturing errors
                    CreateNoWindow = false,         // Show the yt-dlp console window
                    WorkingDirectory = Path.GetDirectoryName(saveFile.FileName) // Set working directory to the folder of the save file
                };

                // Start the process and wait until it finishes.
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                }

                // Notify the user that the download has completed.
                MessageBox.Show("Download completed!");
            }

        }

        private async void DownloadAsMp3(string ytDlpPath, string videoUrl)
        {
            // Step 1: Get the video title safely from the WebView2's current page using JavaScript.
            string pageTitle = await webView21.CoreWebView2.ExecuteScriptAsync("document.title;");

            // The result is returned as a JSON string (with quotes), so remove the surrounding quotes.
            pageTitle = pageTitle.Trim('"');

            // Store the page title as a fallback for the MP3 filename.
            string videoTitle = pageTitle;

            // Step 2: Sanitize the filename to remove invalid characters (preserving Romanian letters if needed).
            videoTitle = SanitizeFileName(videoTitle);

            // Step 3: Show a SaveFileDialog to let the user choose where to save the MP3.
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Save MP3 file",                           // Dialog title
                Filter = "MP3 files|*.mp3|All files|*.*",         // File type filter
                FileName = videoTitle + ".mp3"                    // Default filename
            };

            // If the user cancels the dialog, exit the method early.
            if (saveFile.ShowDialog() != DialogResult.OK)
                return;

            // Step 4: Run yt-dlp to extract audio and save as MP3.
            try
            {
                // Configure ProcessStartInfo to run yt-dlp with arguments for audio extraction.
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath, // Path to yt-dlp executable

                    // Arguments:
                    // --cookies-from-browser firefox → use cookies from Firefox
                    // --no-playlist                  → download only the single video
                    // -x                             → extract audio
                    // --audio-format mp3             → convert audio to MP3
                    // --audio-quality 0              → best quality
                    // "videoUrl"                     → the URL of the video
                    // -o "filename"                  → output file path
                    Arguments = $" --cookies-from-browser firefox --no-playlist -x --audio-format mp3 --audio-quality 0 \"{videoUrl}\" -o \"{saveFile.FileName}\"",

                    UseShellExecute = false,         // Do not use the system shell
                    RedirectStandardOutput = false,  // No need to capture console output
                    RedirectStandardError = false,   // No need to capture errors
                    CreateNoWindow = false,          // Show yt-dlp window
                    WorkingDirectory = Path.GetDirectoryName(saveFile.FileName) // Set working directory to the save folder
                };

                // Start the process and wait until it finishes.
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                }

                // Notify the user that the MP3 download has completed.
                MessageBox.Show("MP3 download completed!");
            }
            catch (Exception ex)
            {
                // If any exception occurs, show an error message to the user.
                MessageBox.Show("Error downloading MP3: " + ex.Message);
            }

        }

        private string SanitizeFileName(string filename)
        {
            // Replace all invalid Windows filename characters with '_'
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
                filename = filename.Replace(c, '_');

            // Replace any character that is NOT:
            // - a-z, A-Z
            // - 0-9
            // - underscore or dash
            // - space
            // - Romanian letters: ăâîșțĂÂÎȘȚ
            filename = Regex.Replace(filename, @"[^a-zA-Z0-9_\- ăâîșțĂÂÎȘȚ]", "_");

            // Collapse multiple underscores into a single underscore
            filename = Regex.Replace(filename, @"_+", "_");

            // Trim leading/trailing whitespace or underscores
            return filename.Trim().Trim('_');
        }

        private bool IsPathPresent(string target)
        {
            // Get the system PATH environment variable (for the whole machine).
            string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            // If PATH is null or empty, return false (target path cannot be present).
            if (string.IsNullOrEmpty(path)) return false;

            // Remove any trailing backslash from the target path to normalize it.
            target = target.TrimEnd('\\');

            // Split the PATH variable by semicolons (each entry is a separate folder),
            // remove empty entries, and check if any entry matches the target path (case-insensitive),
            // also trimming trailing backslashes for each PATH entry.
            return path.Split(';', StringSplitOptions.RemoveEmptyEntries)
                       .Any(p => string.Equals(p.TrimEnd('\\'), target, StringComparison.OrdinalIgnoreCase));

        }

        public static void AddToSystemPath(string folder)
        {
            // Check if the folder exists. If it doesn't, throw an exception.
            if (!Directory.Exists(folder)) throw new DirectoryNotFoundException(folder);

            // Get the system PATH environment variable (for the whole machine).
            // If it's null, use an empty string.
            string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) ?? "";

            // Split PATH into individual entries, remove empty entries, and trim any trailing backslashes.
            var paths = path.Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.TrimEnd('\\'));

            // Check if the folder is already present in the PATH (case-insensitive, trimmed).
            if (!paths.Any(p => string.Equals(p, folder.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
            {
                // If the folder is not in PATH, append it and update the system PATH variable.
                Environment.SetEnvironmentVariable(
                    "PATH", path + ";" + folder, EnvironmentVariableTarget.Machine);

                // Notify other applications that environment variables have changed.
                // HWND_BROADCAST (0xffff) → send message to all top-level windows
                // WM_SETTINGCHANGE (0x1A) → environment variables changed
                // lParam = "Environment"
                // SMTO_ABORTIFHUNG = 2, timeout = 100 ms
                SendMessageTimeout(new IntPtr(0xffff), 0x1A, IntPtr.Zero, "Environment", 2, 100, out _);
            }

        }

        public static void UnzipDeno(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("ZIP file does not exist.");
                return;
            }

            string destinationFolder = Path.GetDirectoryName(zipFilePath); // same folder as the zip

            try
            {
                // Extract to same folder, overwrite if files exist
                ZipFile.ExtractToDirectory(zipFilePath, destinationFolder, true);
                Console.WriteLine($"Extracted {zipFilePath} to {destinationFolder}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error during extraction: {ex.Message}");
            }
        }
    }
}
