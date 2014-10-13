﻿using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenAddingInheritedSubjectNodes
    {
        [Test]
        public async void Can_Add_Base_Type_Nodes_To_Inherited_Type_Pipeline()
        {
            var testNode = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeASub1();

            var pipeline = new PipelineNode<TestObjectASub>();

            pipeline.AddChild(testNode);
            pipeline.AddChild(testNode2);

            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void Can_Add_Base_Type_Pipeline_Node_To_Inherited_Type_Pipeline()
        {
            var testNode = new SimpleTestNodeA1();

            var pipeline = new PipelineNode<TestObjectA>();
            pipeline.AddChild(testNode);

            var testNode2 = new SimpleTestNodeASub1();
            var pipelineSub = new PipelineNode<TestObjectASub>();

            pipelineSub.AddChild(testNode2);
            pipelineSub.AddChild(pipeline);
            
            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void ShouldExecuteFunc_On_Test_Object_Is_Evaluated()
        {
            var testNode = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeASub1();

            testNode2.AddShouldExecute(context => context.Subject.TestValueDecimal == 1);

            var pipeline = new PipelineNode<TestObjectASub>();

            pipeline.AddChild(testNode);
            pipeline.AddChild(testNode2);

            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            testNode2.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

        [Test]
        public async void Can_Add_Inherited_Func_Node_To_Pipleline()
        {
            var testNode = new FuncNode<TestObjectA>();

            testNode.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            testNode.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();

            var pipeline = new PipelineNode<TestObjectASub>();
            pipeline.AddChild(testNode);

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

    }
}