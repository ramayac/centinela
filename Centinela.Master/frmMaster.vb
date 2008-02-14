Imports Centinela.ClassLib
Imports System.IO

Public Class frmMaster
    Public usr As Usuario
    Private datos As ClassLib.AccesoDatos
    Private mab As MinimizarABandeja

    Private sensores As List(Of Sensor)
    Private cargando As Boolean

    Public Notificador As NotificarBandeja.ShellNot = New NotificarBandeja.ShellNot()

    Sub MainFormLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.datos = New AccesoDatos()
        Dim sesion As New frmSesion()
        sesion.ShowDialog(Me)
        Me.usr = sesion.usr
        If Not (Me.usr.GetType().Name = "Administrador") Then
            MessageBox.Show("Solo el Administrador puede iniciar el Servicio Maestro de Centinela.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End If

        Me.txtLog.Text = "Bienvenido Usuario: " + usr.NombreCompleto

        Me.mab = New MinimizarABandeja(Me, "Centinela, Servicio Maestro")
        Dim cfg As New XmlClassLib.ConfigManager("app.config")
        Me.srvPuertos.Interval = CInt(cfg("intervalo"))
    End Sub

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub Restaurar(ByVal mb As System.Windows.Forms.MouseButtons)
        If mb = Windows.Forms.MouseButtons.Left Then
            Me.WindowState = FormWindowState.Maximized
            Try
                Notificador.DelNotifyBox()
            Catch ex As Exception
                'pass
            End Try
        End If
    End Sub

    Public Sub DispararNotificador(ByVal Mensaje As String)
        If Me.WindowState = FormWindowState.Minimized Then
            Notificador.AddNotifyBox(Me.Icon.Handle, Me.Text, "AVISO!", Mensaje)
            Notificador._delegateOfCallBack = New NotificarBandeja.ShellNot.delegateOfCallBack(AddressOf Restaurar)
        End If
    End Sub

    Private Sub ActualizarPuertosASensores(ByVal sender As Object, ByVal e As EventArgs) Handles srvPuertos.Tick
        'If Not (Me.usr Is Nothing) Then
        '    datos.Conectar()
        '    If Me.lstSensores.Items.Count > 0 Then
        '        Me.lblEstado.Text = datos.GetNombreEstadoSensor(Me.sensores(Me.lstSensores.SelectedIndex).Sen.EstadoActual)
        '        datos.Desconectar()
        '    End If
        'End If
    End Sub

    Public Sub CargarSensores()
        datos.Conectar()
        Me.sensores = New List(Of Sensor)
        Me.sensores.Clear() 'Just in case...
        Me.sensores = datos.SelectSensores() 'Todos los sensores
        If (Not sensores Is Nothing) Then
            If (Me.sensores.Count > 0) Then
                'Do something!
            End If
        End If
        datos.Desconectar()
    End Sub

    Private Sub btnComenzar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnComenzar.Click
        Me.srvPuertos.Start()
    End Sub

    Private Sub btnDetener_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDetener.Click
        Me.srvPuertos.Stop()
    End Sub
End Class
