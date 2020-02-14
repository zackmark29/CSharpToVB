﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Imports System.IO
Imports System.Reflection
Imports System.Threading

Imports CSharpToVBApp

Imports CSharpToVBCodeConverter
Imports CSharpToVBCodeConverter.ConversionResult

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit

Imports Xunit

Namespace ConvertDirectory.Tests

    ''' <summary>
    ''' Return False to skip test
    ''' </summary>
    <TestClass()> Public Class TestCompile
        Private _lastFileProcessed As String

        Public Shared ReadOnly Property EnableRoslynTests() As Boolean
            Get
                Return Directory.Exists(GetRoslynRootDirectory)
            End Get
        End Property

        Public Function ProcessFile(PathWithFileName As String, TargetDirectory As String, LanguageExtension As String, CSPreprocessorSymbols As List(Of String), VBPreprocessorSymbols As List(Of KeyValuePair(Of String, Object)), OptionalReferences() As MetadataReference, CancelToken As CancellationToken) As Boolean
            ' Save to TargetDirectory not supported
            Assert.True(String.IsNullOrWhiteSpace(TargetDirectory))
            ' Do not delete the next line or the parameter it is needed by other versions of this routine
            _lastFileProcessed = PathWithFileName
            Using fs As FileStream = File.OpenRead(PathWithFileName)
                Dim RequestToConvert As ConvertRequest = New ConvertRequest(mSkipAutoGenerated:=True, mProgressBar:=Nothing, mCancelToken:=CancelToken) With {
                                                                .SourceCode = GetFileTextFromStream(fs)
                                                                }
                Dim ResultOfConversion As ConversionResult = ConvertInputRequest(RequestToConvert, CSPreprocessorSymbols, VBPreprocessorSymbols, CSharpReferences(Assembly.Load("System.Windows.Forms").Location, OptionalReferences).ToArray, mProgressBar:=Nothing, CancelToken:=CancellationToken.None)
                If ResultOfConversion.ResultStatus = ResultTriState.Failure Then
                    Return False
                End If
                Dim CompileResult As (CompileSuccess As Boolean, EmitResult As EmitResult) = CompileVisualBasicString(StringToBeCompiled:=ResultOfConversion.ConvertedCode, SeverityToReport:=DiagnosticSeverity.Error, ResultOfConversion:=ResultOfConversion)
                If Not CompileResult.CompileSuccess OrElse ResultOfConversion.GetFilteredListOfFailures().Any Then
                    Dim Msg As String = If(CompileResult.CompileSuccess, ResultOfConversion.GetFilteredListOfFailures()(0).GetMessage, "Fatal Compile error")
                    Throw New ApplicationException($"{PathWithFileName} failed to compile with error :{vbCrLf}{Msg}")
                    Return False
                End If
            End Using
            Return True
        End Function

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCodeStyle()

            Dim FilesProcessed As Long = 0

            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "CodeStyle"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCore()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Core"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpCSC()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "CSC"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpCSharpAnalyzerDriver()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "CSharpAnalyzerDriver"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <Timeout(100000)>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpPortable()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Portable"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpTestCommandLine()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Test", "CommandLine"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpTestEmit()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Test", "Emit"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpTestSemantic()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Test", "Semantic"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpTestSyntax()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Test", "Syntax"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersCSharpTestWinRT()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "CSharp", "Test", "WinRT"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersExtension()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Extension"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersRealParserTests()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "RealParserTests"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersServerVBCSCompiler()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Server", "VBCSCompiler"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersServerVBCSCompilerTests()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Server", "VBCSCompilerTests"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersShared()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Shared"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersTest()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "Test"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryCompilersVisualStudio()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Compilers", "VisualBasic"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryDependencies()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Dependencies"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryDeployment()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Deployment"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCore()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "Core"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCoreWpf()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "Core.Wpf"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCSharp()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "CSharp"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCSharpTest()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "CSharp.Test"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCSharpTest2()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "CSharp.Test2"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesCSharpWpf()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "CSharp.Wpf"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesTest()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "Test"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesTest2()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "Test2"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesTestUtilities()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "TestUtilities"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesTestUtilities2()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "TestUtilities2"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesText()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "Text"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesVisualBasic()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "VisualBasic"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryEditorFeaturesVisualBasicTest()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "EditorFeatures", "VisualBasicTest"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryExpressionEvaluator()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "ExpressionEvaluator"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryFeatures()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Features"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryInteractive()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Interactive"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryNuGet()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "NuGet"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryScripting()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Scripting"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectorySetup()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Setup"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryTest()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Test"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryTools()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Tools"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryVisualStudio()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "VisualStudio"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

        <Trait("Category", "SkipWhenLiveUnitTesting")>
        <ConditionalFact(NameOf(EnableRoslynTests))>
        Public Sub ConvertDirectoryWorkspaces()
            Dim FilesProcessed As Long = 0
            Assert.True(ProcessDirectory(Path.Combine(GetRoslynRootDirectory(), "src", "Workspaces"), TargetDirectory:="", MeForm:=Nothing, StopButton:=Nothing, RichTextBoxFileList:=Nothing, LastFileNameWithPath:="", SourceLanguageExtension:="cs", FilesProcessed, TotalFilesToProcess:=0, ProcessFile:=AddressOf ProcessFile, CancellationToken.None), $"Failing file {_lastFileProcessed}")
        End Sub

    End Class

End Namespace