Module Module1

    Sub Main()
        Dim s As Stack(Of Integer)
        Dim link As LinkedListNode(Of Integer)

        Dim d As Func(Of Integer, Integer)
        d = Function(a) a * 3

        Dim str = "ddd"
        Dim q = str.Select(Function(c) c + "3")
        Dim q2 = str.Select(Function(c) c = c + "3")
        For Each item In q2
            Console.WriteLine(item)
        Next

        Dim dt As DateTime
        Dim tbl = New DataTable()
        Dim ls = New List(Of Integer)

        Dim bol = True
        str = Nothing
        Console.WriteLine(bol.ToShiFou)
        Console.WriteLine(str.IsNullOrEmpty)
        Console.ReadKey()
    End Sub

    Sub FVar()
        Dim arrStr1() As String = {"33", "44", "55"}
        Dim q1 = From i In arrStr1
                 Select i.ToUpper

        Dim q2 = arrStr1.Select(Function(s) s = s + "1")
        Dim q3 = arrStr1.Select(Function(s)
                                    s += "1"
                                    Return s.ToUpper
                                End Function)

        Dim var1 = New With {.str = "33",
                             .i = 2}

    End Sub

    Function F1(Of T As IComparable(Of T))() As T

    End Function

    Iterator Function F2() As IEnumerable(Of Integer)
        Yield 1
        If Date.Now.Day Mod 2 = 1 Then Exit Function
        Yield 2
    End Function

    Iterator Function MyFunc() As IEnumerable(Of Integer)
        For index = 1 To 10
            Yield index
            Exit For
        Next
        Exit Function
        Yield 0
    End Function

End Module
