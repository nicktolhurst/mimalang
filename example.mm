
hello <| 'Hello'
'World!' |> world

checkShouldPrint? <| (bool) if:true:true?false |> shouldPrint

do checkShouldPrint?


if  :(do checkShouldPrint?)
    :(do printHelloWorld) 

printHelloWorld <| (nil) [arg1 arg2] prnt:[arg1 arg2]


if shouldPrint? do printHelloWorld ('Hello' 'World')


func <| (int) [arg1 arg2] method:[args] |> message



// Language Definitions
//
// Data Structures
//
//  - Immutable
//  - Value equality semantics
//  - Collections:
//      - Sequensable 
//      - Iterable
//  - Nullable
//
//  Numbers
//
//  - int, float/dec, frac

integer <|  int`123     // full syntax
        ||  i`123       // shorthand syntax
        ||  `123        // inferred type
        ||  123         // inferred type 
        ||  `1/2+`1/2   // resolves to integer

quarter <|  frc`1/4     // full sytax
        ||  f`1/4       // shorthand syntax
        ||  f`0.25      // explicit conversion
        ||  `1/4        // inferred type

decimal <|  dec`1.25    // full
        ||  d`1.25      // shorthand
        ||  1.25        // inferred
        ||  1`1/4       // implicit
        ||  1/4         // implicit (evaluation)




// Functoins / Pipelines

// call a simple functionm store the result
#result <| a_thing 

// call a simple function but discard the result
<> a_thing

// call a simple function that does not return a result
a_thing

// call a function with parameters and store the result
#result <| to_dec <| frc`1/4

// doesn't matter which way around these are

a_thing |> #result
a_thing <>
a_thing
frc`1/4 |> to_dec |> #result

// chaining functions

i`result <| | do round_floor
            | do get_first
            | do sort_oldest
            | <| [1.3 1/5 12 1`1/5]


// try/catch


a_result <| | do round_floor
            | try   | do get_first
                    | do sort_oldest
                    | catch ['null_ref_exception']      |> throw 'Null Reference Exception'
                    | catch ['out_of_range_exception']  |> throw 'Argument Out Of Range Exception'
            | <| [1.3 1/5 12 1`1/5]



// if/else

a_result <| | if condition  
                | do this
                | do that
            | else  
                | if another_condition
                    | do this
                    | do that
                | else
                    | do this
            
// while
<>  | while condition
        | do this
        | do that
        | another_condition <|  | do some_work
                                | do return_condition
        | if another_condition
            | condition = false


// for

<>  | for x in Collection
        | do this_with_param x

<>  | for x i in Collection
        | do this_with_param x
        | do this_with_index i

<>  | for i<|0 i>=100 i++
        | do this_with_index i



// functions 
my_func <| (int) [arg1 arg2] do something

// list/set
[1 2 5 7 1]

// map/dict
{a:1 b:4 c:6}

// can be mixed and matched
{a:{aa:1 ab:2} b:4 c:[1 3 4 5]}

// everything is immutable, postfix with <|| to allow overwriting


my_set <| [1 2 3 4 5]

my_set <| [3 5 6 7] // throws error

my_mutable <|| [4 5 6 7 8] // forces mutation

