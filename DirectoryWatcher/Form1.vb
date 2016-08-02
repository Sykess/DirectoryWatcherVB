Imports System.IO



Public Class Form1
    Dim myFileCompare As New FileCompare
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NotifyIcon1.Icon = Icon
        SaveFileDialog1.InitialDirectory = Application.StartupPath
        SaveFileDialog1.FileName = "DisplayLog" & Date.Now.Month & Date.Now.Day & ".txt"

    End Sub

    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        StatusBox.Text = "Now watching for changes, started at " & Date.Now & vbCrLf
        SaveLogAsToolStripMenuItem.Enabled = True
        SaveLogToolStripMenuItem.Enabled = True
        StartButton.Enabled = False
        StopButton.Enabled = True
        MyWatcher.EnableRaisingEvents = True
        'Run()
    End Sub

    Private Sub StopButton_Click(sender As Object, e As EventArgs) Handles StopButton.Click
        If DisableStopButton.Checked = False Then
            Dim answer = MessageBox.Show("Would you like to Save the log?", "Stop Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If answer = MsgBoxResult.Yes Then
                SaveLogAsToolStripMenuItem_Click(sender, e)
            End If
        End If
        StatusBox.Text = "Click Start to begin watching again!" & vbCrLf
        StartButton.Enabled = True
        StopButton.Enabled = False
        SaveLogAsToolStripMenuItem.Enabled = False
        SaveLogToolStripMenuItem.Enabled = False
        MyWatcher.EnableRaisingEvents = False
    End Sub

    ' Test commit

    Private Sub MyWatcher_Renamed(sender As Object, e As RenamedEventArgs) Handles MyWatcher.Renamed
        If e.Name.Contains("xlsx") Then
            Dim copyFrom As String = e.FullPath
            Dim copyTo As String = "\\alliance2.gd-ais.com\DavWWWRoot\sites\3\WCS\WCS Shared Documents IPT WCS Software\TI14 APB15\Workarea\RAN PTR Effort\" & e.Name
            StatusBox.AppendText("Time: " & Date.Now & vbCrLf)
            StatusBox.AppendText("File changed: " & e.Name & vbCrLf)
            Try
                File.Delete(copyTo)
                File.Copy(copyFrom, copyTo)
            Catch ex As Exception
                StatusBox.AppendText("Error copying file " & e.Name & vbCrLf)
            End Try
        End If
    End Sub

    Private Sub CompareButton_Click(sender As Object, e As EventArgs) Handles CompareButton.Click
        Dim alliance As String = "\\alliance2.gd-ais.com\DavWWWRoot\sites\3\WCS\WCS Shared Documents IPT WCS Software\TI14 APB15\Workarea\RAN PTR Effort\"
        Dim yDrive As String = "Y:\WCS\PTR Phase Eternity\"
        If Directory.Exists(alliance) AndAlso Directory.Exists(yDrive) Then
            CompareButton.Enabled = False
            If DisableForceUpdates.Checked = False Then
                Dim answer = MessageBox.Show("Are you sure you want to overwrite Alliance files?", "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If answer = MsgBoxResult.No Then
                    CompareButton.Enabled = True
                    Return
                End If
            End If
            Try
                Dim dirY As New DirectoryInfo(yDrive)
                Dim listY = dirY.GetFiles("*PTR*.xlsx", SearchOption.TopDirectoryOnly)
                For Each Cfile In listY
                    If Cfile.Name = "" Or Cfile.Name.Contains("$") Then
                        Continue For
                    End If
                    File.Delete(alliance & Cfile.Name)
                    File.Copy(yDrive & Cfile.Name, alliance & Cfile.Name)
                Next
                    MessageBox.Show("Alliance updated successfully")
                Catch ex As Exception
                    MsgBox("Error copying files")
                Finally
                    CompareButton.Enabled = True
                End Try
            Else
                MessageBox.Show("Either the Alliance or the Y drive folder doesn't exist")
        End If

    End Sub

    Private Function md5(ByVal file_name As String) As String
        'Dim MSXML As MSXML2.DOMDocument = New MSXML2.DOMDocument
        Dim fileBytes As Byte() = File.ReadAllBytes(file_name)
        Dim shaM As New Security.Cryptography.SHA512Managed()
        Dim shaB As Byte() = shaM.ComputeHash(fileBytes)

        Dim sBuilder As New System.Text.StringBuilder()
        Dim i As Integer
        For i = 0 To shaB.Length - 1
            sBuilder.Append(shaB(i).ToString("x2"))
        Next
        Return sBuilder.ToString()
    End Function

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        If WindowState = FormWindowState.Minimized Then
            Show()
            WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If WindowState = FormWindowState.Minimized Then
            Hide()
        End If
    End Sub

    Public Class FileCompare
        Implements Collections.Generic.IEqualityComparer(Of System.IO.FileInfo)

        Public Function Equals1(x As FileInfo, y As FileInfo) As Boolean Implements IEqualityComparer(Of FileInfo).Equals
            If x.Name = y.Name Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetHashCode1(obj As FileInfo) As Integer Implements IEqualityComparer(Of FileInfo).GetHashCode
            Dim s As String = obj.Name
            Return s.GetHashCode()
        End Function
    End Class

    Private Sub SaveLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveLogToolStripMenuItem.Click
        Dim currentPath As String = ""
        Try
            'currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            'currentPath = New Uri(currentPath).LocalPath
            currentPath = Application.StartupPath
            currentPath += "DisplayLog" & Date.Now.Month & Date.Now.Day & ".txt"
            My.Computer.FileSystem.WriteAllText(currentPath, StatusBox.Text & vbCrLf, True)
            MessageBox.Show(currentPath & " Saved Successfully!")
        Catch ex As Exception
            MessageBox.Show("Error saving to " & currentPath)
        End Try
    End Sub

    Private Sub SaveLogAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveLogAsToolStripMenuItem.Click
        Dim saveFile As String = ""
        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            saveFile = SaveFileDialog1.FileName
            Try
                My.Computer.FileSystem.WriteAllText(saveFile, StatusBox.Text & vbCrLf, True)
                MessageBox.Show(saveFile & " Saved Successfully!")
            Catch ex As Exception
                MessageBox.Show("Error saving to " & saveFile)
            End Try
        End If
    End Sub
End Class



