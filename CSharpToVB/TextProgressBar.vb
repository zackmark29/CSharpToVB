﻿Imports System.Drawing.Drawing2D
Imports System.Threading

Public Class TextProgressBar
    Implements IProgress(Of ProgressReport)

    Private ReadOnly _defaultFont As Font = New Font("Segoe UI", 8, FontStyle.Bold)
    Private ReadOnly _progressBar As ToolStripProgressBar

    Public Sub New(Bar As ToolStripProgressBar)
        _progressBar = Bar
        _progressBar.TextImageRelation = TextImageRelation.Overlay
        _progressBar.Value = 0
    End Sub

    Private Sub pbPrecentage(pb As ToolStripProgressBar, Text As String)
        pb.Invalidate()
        Using g As Graphics = pb.ProgressBar.CreateGraphics()
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed
            'Switch to Anti-aliased drawing for better (smoother) graphic results
            g.SmoothingMode = SmoothingMode.AntiAlias
            Dim sizeF As SizeF = g.MeasureString(Text, _defaultFont)
            g.DrawString(Text, _defaultFont, Brushes.Black, New PointF(pb.Width \ 2 - (sizeF.Width / 2.0F), pb.Height \ 2 - (sizeF.Height / 2.0F)))
        End Using
    End Sub

    Public Sub Clear()
        With _progressBar
            .Value = 0
            .Visible = False
        End With
    End Sub

    Public Sub Maximum(Value As Integer)
        With _progressBar
            .Visible = True
            .Maximum = Value
        End With
    End Sub

    ''' <summary>
    ''' Advances the current position of the underlying Progress Bar by the specified amount.
    ''' </summary>
    ''' <param name="Value">The amount by which to increment the underlying progress bar's current position.</param>
    Public Sub Increment(Value As Integer)
        With _progressBar
            .Increment(Value)
            pbPrecentage(_progressBar, $"{ .Value:N0} of { .Maximum:N0}")
            Thread.Sleep(1)
            If .Value >= .Maximum Then
                .Visible = False
            End If
        End With
    End Sub

    Public Sub Report(value As ProgressReport) Implements IProgress(Of ProgressReport).Report
        With _progressBar
            .Visible = value.Maximum = 0 OrElse value.Current < value.Maximum
            .Maximum = value.Maximum
            .Value = value.Current
        End With

        pbPrecentage(_progressBar, $"{value.Current:N0} of {value.Maximum:N0}")
    End Sub

End Class