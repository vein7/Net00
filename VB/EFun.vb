Imports System.Runtime.CompilerServices

Module Ext

    <Extension>
    Public Function ToShiFou(b As Boolean) As String
        Return IIf(b, "是", "否")
    End Function

    <Extension>
    Public Function IsNullOrEmpty(str As String) As Boolean
        Return str Is Nothing OrElse str.Length = 0
    End Function

End Module