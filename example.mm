// Examples on how we handle namespace imports / modules / libraries (name to be confirmed).
// This example includes the verbose, one-liners, and shorthand variations.
// load() is a built in function that loads files from disk.
// import() is a built in function that imports public map keys into the runtime.
// :: is an import specific token.

// Import everything from a file:
    
    // verbose 
    imports <| load('some/namespace') 
    import(imports)

    // one-liner
    import(<| load('some/namespace'))

    // shorthand
    ::some/namespace

    // Functions can now be accessed directly
    func_a(property)
    func_b('example')

// Import only specific map keys from a file:

    // verbose
    imports <|  | filter_keys [func_a func_b property] 
                | load 'some/namespace'
    import(imports) 

    // one-liner
    import(<| import | filter_keys([func_a func_b property]) | load('some/namespace'))

    // shorthand 
    ::some/namespace | [func property etc]

// load specific keys from namespace but also change key name (alias)

    // TODO: verbose & one-liner

    // shorthand
    ::some/namespace | {a:func b:property c:etc}


// shorthand translation
::some/namespace
// translates to
do #( map
        #[ ns ] 
        #[ ns map ] 
        #[ ns arr ] <|   
            | if arr != nil 
                | filter_keys ( arr ) 
            | if map != nil
                | map_map ( map ) 
            | ns )


// object name will be the class name:
class <| {
    // fields are a special map
    f: {a: nil b: nil}

    // constructors are a list, allowing for polymorphism. function name can be anything.
    c: [ #(#[a, b] | f[a] <| a && f[b] <| b) ]

    // keys can contain any data structure, including functions or other classes. 
    fn: #(void #[c] do_something(c))

    a: 'a'

    b: 1
}

instance <| class::c(1 2)
// {
//     instance = {
//         f: {
//             a: 1
//             b: 2
//         }
//         c: [
//             //fn...
//         ]
//         fn: //fn...
//         a: 'a'
//         b: 1
//     }
// }

instance::fn('123')






// functions: #(type #[args ..] <| piped | functions | right | to | left)
//
print_sorted <| #(void #[list] <| printf($) | sort | list)

this::print_sorted [1 12 5 33 0 63] // or
do print_sorted [1 12 5 33 0 63]

// classes are just objects: name <| { prop: val } where a property can return a type of self constructred.
//
dog <| {
    // fields could be a nested map - or not ¯\_(ツ)_/¯ 
    fields: {name: nil breed: nil}

    // Constructors are a list, allowing for polymorphism by matching signitures. The key for the
    // constructor prorperty could be anything.
    rescue: [dog #(#[name, breed] | fields[name] <| name && fields[breed] <| breed) ]

    // keys can contain any data structure, including functions or other classes. 
    train: #(bool #[command] <| | random | [true false])

    // expose fields as properties for a public api feel.
    name: fields[name]
}

// calling an array of functions will attempt to match the signiture. This returns a dog object with fields set.
pet <| dog::rescue('Mima' 'Mongrel')

// calling the train method returns a bool. Random chance of this been a success.
isTrained <| pet::train('sit')

// can access name propery if mapped in object.
pet_name <| pet.name 

// if not mapped, this would be: 
pet_name <| pet.fields[name] 





// example of how a class could work as a custom object object









// name args setter
@(my_class #[arg1 arg2] <| 
{
    {
        field_a, 
        field_b
    }: 
    [
        #(my_class #[arg1 arg2] | field_a <| arg1 && field_b <| arg2 | return my_class)
        #(my_class #[arg1] | my_class arg1 4 | return my_class)
    ]

    public_method_args: <| #(int #[arg1 arg2] | do private_method arg1 arg2) 

    public_method_fields <| #(int | return field_a + field_b)

    @private_method: <| #(int #[arg1 #arg2] | return arg1 + arg2)
})

objcet_a <| new my_class
result_a <| do object_a.public_method_args 3 5

result_b <| (new my_class).public_method_args 22 27

// result_a is 8
// result_b is 49







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

