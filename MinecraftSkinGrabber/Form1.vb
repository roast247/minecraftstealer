Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq

Public Class Form1
    Dim request As HttpWebRequest
    Dim response As HttpWebResponse
    Dim reader As StreamReader
    Dim rawresp As String
    Dim uid As String
    Dim base64 As String
    Dim base64Decoded As String
    Dim data() As Byte
    Dim SkinLink As String
    Dim reqlink As String
    Dim drag As Boolean
    Private draggable As Boolean
    Private mouseX As Integer
    Private mouseY As Integer

    Private Sub SaveFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        If HaveInternetConnection() Then
            If File.Exists(SaveFileDialog1.FileName) Then
                File.Delete(SaveFileDialog1.FileName)
            End If
            My.Computer.Network.DownloadFile(PictureBox1.ImageLocation, SaveFileDialog1.FileName)
            SaveFileDialog1.FileName = "Minecraft Skin"
        Else
            MsgBox("Could not connect to the Minecraft servers", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim parts() As String = Split(My.User.Name, "\")
        Dim username As String = parts(1)
        SaveFileDialog1.InitialDirectory = ("C:\users\" + username + "\Pictures\")
        PictureBox1.SizeMode = PictureBoxSizeMode.CenterImage

        With TextBox1
            .SelectionStart = .TextLength
            .SelectionLength = 0
            .SelectionStart = 0
            .ScrollToCaret()
        End With



    End Sub

    Public Function HaveInternetConnection() As Boolean
        Try
            Return My.Computer.Network.Ping("www.minecraft.net")
        Catch
            Return False
        End Try
    End Function

    Public Sub MakeRequest(reqlink)
        request = DirectCast(WebRequest.Create(reqlink), HttpWebRequest)
        response = DirectCast(request.GetResponse(), HttpWebResponse)
        reader = New StreamReader(response.GetResponseStream())
        rawresp = reader.ReadToEnd()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        If PictureBox1.Image Is Nothing Then
            MsgBox("Please find a skin first", MsgBoxStyle.Critical, "Error")
        Else
            SaveFileDialog1.ShowDialog()
        End If
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        If HaveInternetConnection() Then
            If TextBox1.Text = "" Then
                MsgBox("Please Enter A Username In The Text Box", MsgBoxStyle.Critical, "Error")
            Else
                Try
                    MakeRequest("https://api.mojang.com/users/profiles/minecraft/" & TextBox1.Text)
                    uid = JObject.Parse(rawresp)("id")
                    MakeRequest("https://sessionserver.mojang.com/session/minecraft/profile/" & uid)
                    base64 = JObject.Parse(rawresp)("properties")(0)("value")
                    data = Convert.FromBase64String(base64)
                    base64Decoded = System.Text.Encoding.ASCII.GetString(data)
                    SkinLink = JObject.Parse(base64Decoded)("textures")("SKIN")("url")
                    PictureBox1.ImageLocation = SkinLink
                Catch wex As WebException
                    Try
                        response = DirectCast(wex.Response, HttpWebResponse)
                        MsgBox("HTTP Code " & response.StatusCode & ". Description: " & wex.Message & " You can look up this code online for more info.", MsgBoxStyle.Information, "Web Request Error")
                    Catch ex As Exception
                        MsgBox("Timed out, the program will restart after this", MsgBoxStyle.Information, "Error")
                        Application.Restart()
                    End Try
                Catch ex As Exception
                    MsgBox("User does not exist or the Minecraft API is not returning a link to the skin image", MsgBoxStyle.Exclamation, "Error")
                End Try
            End If
        Else
            MsgBox("Could not connect to the Minecraft servers", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged_1(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        Else
            PictureBox1.SizeMode = PictureBoxSizeMode.CenterImage
        End If
    End Sub

    Private Sub KryptonButton1_Click(sender As Object, e As EventArgs) Handles KryptonButton1.Click
        Close()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        With TextBox1
            If .Text = "" Then
                .Text = "Username"
                .ForeColor = Color.Gray
            End If
            If .Text = "Username" And .ForeColor = Color.Gray Then
                .ShortcutsEnabled = False
            Else
                .ShortcutsEnabled = True
            End If
            If .TextLength > 8 Then
                If StrReverse(StrReverse(.Text).Remove(8)) = "Username" Then
                    .Text = .Text.Remove(.TextLength - 8)
                    .ForeColor = Color.Black
                    .SelectionStart = .TextLength
                    .ScrollToCaret()
                End If
            End If
        End With
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        With TextBox1
            If .Text = "Username" And .ForeColor = Color.Gray Then
                If e.KeyCode = Keys.Right Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Home Or e.KeyCode = Keys.End Then
                    e.Handled = True
                End If
            End If
        End With
    End Sub

    Private Sub TextBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseDown
        drag = True
        With TextBox1
            If .Text = "Username" And .ForeColor = Color.Gray Then
                .SelectionStart = .TextLength
                .SelectionLength = 0
                .SelectionStart = 0
                .ScrollToCaret()
            End If
        End With
    End Sub

    Private Sub TextBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseMove
        If drag Then
            With TextBox1
                If .Text = "Username" And .ForeColor = Color.Gray Then
                    TextBox1.Select(0, 0)
                End If
            End With
        End If
    End Sub

    Private Sub TextBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseUp
        drag = False
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub
    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        draggable = False
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
    Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) Handles Label1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Label1_MouseMove(sender As Object, e As MouseEventArgs) Handles Label1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Label1_MouseUp(sender As Object, e As MouseEventArgs) Handles Label1.MouseUp
        draggable = False
    End Sub

    Private Sub Label2_Paint(sender As Object, e As PaintEventArgs) Handles Label2.Paint

    End Sub

    Private Sub Label2_MouseDown(sender As Object, e As MouseEventArgs) Handles Label2.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Label2_MouseMove(sender As Object, e As MouseEventArgs) Handles Label2.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Label2_MouseUp(sender As Object, e As MouseEventArgs) Handles Label2.MouseUp
        draggable = False
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub

    Private Sub PictureBox2_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub PictureBox2_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub PictureBox2_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseUp
        draggable = False
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        draggable = False
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        draggable = True
        mouseX = Cursor.Position.X - Me.Left
        mouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If draggable Then
            Me.Top = Cursor.Position.Y - mouseY
            Me.Left = Cursor.Position.X - mouseX
        End If
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        draggable = False
    End Sub
End Class
