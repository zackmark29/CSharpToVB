﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.CompilerServices
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Classification
Imports Microsoft.CodeAnalysis.CSharp.Formatting
Imports Microsoft.CodeAnalysis.Formatting
Imports Microsoft.CodeAnalysis.Options
Imports Microsoft.CodeAnalysis.Text

Public Module RangeSupport

    <Extension>
    Private Function AdjustAdditiveSpans(spans As IEnumerable(Of ClassifiedSpan)) As List(Of ClassifiedSpan)
        Dim newSpans As New List(Of ClassifiedSpan)
        Dim spanEnd As Integer
        For i As Integer = 0 To spans.Count - 1
            Dim span As ClassifiedSpan = spans(i)
            If span.TextSpan.Start < spanEnd Then
                Continue For
            End If
            If ClassificationTypeNames.AdditiveTypeNames.Contains(span.ClassificationType) Then
                Continue For
            End If
            If span.ClassificationType <> "string - escape character" Then
                newSpans.Add(span)
                spanEnd = span.TextSpan.End
            Else
                Stop
            End If
        Next
        Return newSpans
    End Function

    Private Iterator Function FillGaps(text As SourceText, ranges As IEnumerable(Of Range)) As IEnumerable(Of Range)
        Const whitespaceClassification As String = Nothing

        Dim current As Integer = 0
        Dim previous As Range = Nothing

        For Each range As Range In ranges
            Dim start As Integer = range.TextSpan.Start
            If start > current Then
                Yield New Range(whitespaceClassification, TextSpan.FromBounds(current, start), text)
            End If

            If previous Is Nothing OrElse range.TextSpan <> previous.TextSpan Then
                Yield range
            End If

            previous = range
            current = range.TextSpan.End
        Next

        If current < text.Length Then
            Yield New Range(whitespaceClassification, TextSpan.FromBounds(current, text.Length), text)
        End If
    End Function

    <Extension>
    Friend Function GetForwardItem(Of T)(ListOfT As List(Of T), index As Integer, LookAhead As Integer) As T
        Dim finalIndex As Integer = index + LookAhead
        If finalIndex < 0 Then
            Return Nothing
        End If
        Return If(finalIndex < ListOfT.Count, ListOfT(finalIndex), Nothing)
    End Function

    Public Function GetClassifiedRanges(SourceCode As String, Language As String) As IEnumerable(Of Range)
        Using workspace As New AdhocWorkspace()
            Dim solution As Solution = workspace.CurrentSolution
            Dim document As Document
            If Language = LanguageNames.CSharp Then
                Dim project As Project = solution.AddProject("projectName", "assemblyName", LanguageNames.CSharp)
                document = project.AddDocument("name.cs", SourceCode)

                Dim csOptions As OptionSet = workspace.Options
                csOptions = csOptions.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, value:=True)
                csOptions = csOptions.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, value:=True)
            Else
                Dim vbOptions As OptionSet = workspace.Options
                Dim project As Project = solution.AddProject("projectName", "assemblyName", LanguageNames.VisualBasic)
                document = project.AddDocument("name.vb", SourceCode)
            End If

            document = Formatter.FormatAsync(document).Result
            Dim text As SourceText = document.GetTextAsync().Result
            Dim classifiedSpans As List(Of ClassifiedSpan) = Classifier.GetClassifiedSpansAsync(document, TextSpan.FromBounds(0, text.Length)).Result.AdjustAdditiveSpans
            Dim ranges As IEnumerable(Of Range) = From span As ClassifiedSpan In classifiedSpans
                                                  Select New Range(span, text.GetSubText(span.TextSpan).ToString())
            ' Whitespace isn't classified so fill in ranges for whitespace.
            Return FillGaps(text, ranges)
        End Using
    End Function

End Module
