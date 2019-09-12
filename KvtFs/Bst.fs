module Bst
    type BST<'T> =
        | Empty
        | Node of value:'T * left: BST<'T> * right: BST<'T>

    let rec exists item bst =
        match bst with
        | Empty -> false
        | Node (x, left, right) ->
            if item = x then true
            elif item < x then (exists item left) // Check the left subtree.
            else (exists item right) // Check the right subtree.

    let rec insert item bst =
        match bst with
        | Empty -> Node(item, Empty, Empty)
        | Node(x, left, right) as node ->
            if item = x then node // No need to insert, it already exists; return the node.
            elif item < x then Node(x, insert item left, right) // Call into left subtree.
            else Node(x, left, insert item right) // Call into right subtree.
    
    
