using System;
using System.Collections.Generic;
using Xunit;

namespace lqd.net.domain.results.tests {

    public class ResultErrorTests {

        // (done) has on a collection of result errors will return true if it contains a specific result error
        // (done) has on a collection of result errors will return true if it does not contain a specific result error
        //  it is invalid to pass a null collection into has


        [Fact]
        public void has_on_a_collection_of_result_errors_will_return_true_if_it_contains_a_specific_result_error() {
            var errors = new [] { new TestError() };

            Assert.True(  errors.Has<TestError>() );
        }

        [Fact]
        public void has_on_a_collection_of_result_errors_will_return_true_if_it_does_not_contain_a_specific_result_error() {
            var errors = new ResultError [] { }; 

            Assert.False( errors.Has<TestError>() );

        }

        [Fact]
        public void it_is_invalid_to_pass_a_null_collection_into_has() {

            var errors = (IEnumerable<ResultError>)null;

            Assert.Throws<ArgumentNullException>( () =>  errors.Has<TestError>()  );

        }

        public class TestError : ResultError { }

    }
}
