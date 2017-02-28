using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lqd.net.domain.results.tests {

    public class AddResultTests {

        /*
            return validator
                     .Validate( command )
                     .Then( factory.Create )
                     .Then( command.Repository.Add );
        */

        // (done) can create a success result
        // (done) can create an error result from a single error
        // (done) can create an error result from multiple errors
        
        // (done) the success details are passed into the success action
        // (done) the errors are passed into the error action
        // (done) the error is passed into the error action

        // (done) passing a null object as a success result is not a valid use
        // (done) passing a null error as an error result is not a valid use
        // (done) passing a null collection of errors as an error result is not a valid use
        // (done) passing an empty collection of errors as the error result is not valid
        // (done) passing a null success action to match is not a valid use 
        // (done)passing a null error action to match is not a valid use

        // (done) then will apply the transformation if it is already a succcess
        // (done) then will not apply the transformation if it is already an error
        // (done) passing an null then function is not valid 

        // (done) then with an action that accepts no arguments will apply the action if it is a success
        // (done) then with an action that accepts no arguments will ignore the action if it is not a success


        // The async

        //  :: AR<P> -> ( P -> T<Q> ) -> T<AR<Q>>
        //  :: T<AR<P>> -> ( P -> T<Q> ) -> T<AR<Q>>
        //  :: T<AR<P>> -> ( P -> Q ) -> T<AR<Q>>

        // (done) async then will return a task if it is a success 
        // (done) async then will return a task if it is an error

        [Fact]
        public void can_create_a_success_result() {

            var details = new object();

            var result = AddResult<object>.WasSuccess( details  );


            result
              .Match(
                success: value => Assert.True( true ),
                error: errors => Assert.False( true ) 
              );
        }

        [Fact]
        public void can_create_an_error_result_from_single_errors() {

            var error = new TestError();
            var result = AddResult<object>.WasError( error );


            result
                .Match(
                    success: value => Assert.False( true ),
                    error: errors => Assert.True( true )
                );

        }

        [Fact]
        public void can_create_an_error_result_from_a_multiple_errors() {
            var errors = new [] { new TestError(), new TestError() };
            
            var result = AddResult<object>.WasError( errors );


            result
                .Match(
                    success: details => Assert.False( true ),
                    error: errs => Assert.True( true ) 
                );

        }

        [Fact]
        public void the_success_details_are_passed_into_the_success_action() {

            var details = new object();
            var result = AddResult<object>.WasSuccess( details );

            result.
                Match(
                    success: value => Assert.Equal( details, value ),
                    error: errors => Assert.False( true )   
                );


        }

        [Fact]
        public void the_errors_are_passed_into_the_error_action() {

            var errors = new [] { new TestError(), new TestError() };
            var result = AddResult<object>.WasError( errors );

            result
                .Match( 
                    success: value => Assert.False( true ),
                    error: errs => Assert.Equal( errors, errs )  
                );

        }

        [Fact]
        public void the_error_is_passed_into_the_error_action() {

            var error = new TestError();
            var result = AddResult<object>.WasError( error );

            result
                .Match(  
                    success: value => Assert.True( false ),
                    error: errs => Assert.True( errs.Contains( error ) )   
                );

        }

        [Fact]
        public void passing_a_null_object_as_a_success_result_is_not_a_valid_use() {

            var act = (Action)(() => AddResult<object>.WasSuccess( null ));

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_error_as_an_error_result_is_not_a_valid_use() {

            var error = (ResultError)null;
            var act = (Action)( () => AddResult<object>.WasError( error )  );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_collection_of_errors_as_an_error_result_is_not_a_valid_use() {

            var errors = (IEnumerable<ResultError>)null;
            var act = (Action)( () => AddResult<object>.WasError( errors ) );


            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_an_empty_collection_of_errors_as_the_error_result_is_not_valid() {

            var errors = Enumerable.Empty<ResultError>();
            var act = (Action)( () => AddResult<object>.WasError( errors ) );


            Assert.Throws<ArgumentException>( act );

        }

        [Fact]
        public void passing_a_null_success_action_to_match_is_not_a_valid_use() {

            var result = AddResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( null, errors => { } ) );


            Assert.Throws<ArgumentNullException>( act );


        } 

        [Fact]
        public void passing_a_null_error_action_to_match_is_not_a_valid_use() {

            var result = AddResult<object>.WasSuccess( new object() );
            var act  = (Action)(() => result.Match( v => { }, null ));


            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_success_function_to_match_is_not_a_valid_use() {

            var result = AddResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( null, errors => errors ) );


            Assert.Throws<ArgumentNullException>( act );
        } 

        [Fact]
        public void passing_a_null_error_function_to_match_is_not_a_valid_use() {

            var result = AddResult<object>.WasSuccess( new object() );
            var act  = (Action)(() => result.Match( v => v, null ));


            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void then_will_apply_the_transformation_if_it_is_already_a_succcess() {

            var expected = new object();
            var result = AddResult<object>.WasSuccess( new object() );


            result
                .Then( v => expected )
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    error: errs => Assert.False( true )
                );

        }

        [Fact]
        public void then_will_not_apply_the_transformation_if_it_is_already_an_error() {

            var result = AddResult<object>.WasError( new TestError() );


            result
                .Then<object>( value => { throw new Exception(); } );

        }

        [Fact]
        public void passing_an_null_then_function_is_not_valid() {

            var result = AddResult<object>.WasSuccess( new object() );
            var act =(Action)(() => result.Then<object>( null )  );


            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public async void async_then_will_return_a_task_if_it_is_a_success() {

            var expected = new object();
            var result = AddResult<object>.WasSuccess( new object() );


            var taskResult = await result
                                    .ThenAsync( v => Task.FromResult( expected ));

            taskResult
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    error: errors => Assert.False( true )  
               );

        }

        [Fact]
        public async void async_then_will_return_a_task_if_it_is_an_error() {

            var error = new TestError();
            var result = AddResult<object>.WasError( error );

            var taskResult = await result
                                    .ThenAsync( v => Task.FromResult( new object()  ));


            taskResult
                .Match(
                    success: value => Assert.False( true ),
                    error: errors => Assert.True( errors.Contains( error ))   
                 );
        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_apply_the_action_if_it_is_a_success() {

            var action_applied = false;
            var result = AddResult<object>.WasSuccess( new object() );  


            result
                .Then( () => action_applied = true );
                

            Assert.True( action_applied );

        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_ignore_the_action_if_it_is_not_a_success() {

            var action_applied = false;
            var result = AddResult<object>.WasError( new TestError() );

            result
                .Then( () => action_applied = true );


            Assert.False( action_applied );
        }

        public class TestError : ResultError { }

    }



}
