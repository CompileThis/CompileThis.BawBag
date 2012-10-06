namespace CompileThis.BawBag.Core.Plugins.RandomChoices
{
    using CompileThis.BawBag.Jabbr;
    using FluentAssertions;
    using Moq;
    using Xunit;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Extensibility.Internal;
    using CompileThis.BawBag.Plugins.RandomChoices;

    public class ChooseHandlerFacts
    {
        [Fact]
        public void ChooseHandler_properties_are_correct()
        {
            var handler = new ChooseHandler();

            handler.Name.Should().Be("Choose");
            handler.Priority.Should().Be(PluginPriority.Normal);
            handler.ContinueProcessing.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_action_messages_are_ignored()
        {
            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "a or b",
                    Type = MessageType.Action
                };

            var context = new PluginContext();

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_broadcast_messages_are_ignored()
        {
            var handler = new ChooseHandler();

            var message = new Message
            {
                Text = "a or b",
                Type = MessageType.Broadcast
            };

            var context = new PluginContext();

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_private_messages_are_ignored()
        {
            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "a or b",
                    Type = MessageType.Private
                };

            var context = new PluginContext();

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_unaddressed_mismatches_are_ignored()
        {
            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "a or b",
                    Type = MessageType.Default
                };

            var context = new PluginContext();

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_addressed_mismatches_are_ignored()
        {
            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "a",
                    Type = MessageType.Default
                };

            var context = new PluginContext
                {
                    IsBotAddressed = true
                };

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }

        [Fact]
        public void ChooseHandler_addressed_matches_are_handled()
        {
            var randomMock = new Mock<IRandomNumberProvider>();

            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "choose a or b",
                    Type = MessageType.Default
                };

            var context = new PluginContext
                {
                    IsBotAddressed = true,
                    User = new User
                        {
                            Name = "TestUser",
                        },
                    RandomProvider = randomMock.Object
                };

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeTrue();
        }

        [Fact]
        public void ChooseHandler_unaddressed_matches_are_ignored()
        {
            var randomMock = new Mock<IRandomNumberProvider>();

            var handler = new ChooseHandler();

            var message = new Message
                {
                    Text = "choose a or b",
                    Type = MessageType.Default
                };

            var context = new PluginContext
                {
                    IsBotAddressed = false,
                    User = new User
                        {
                            Name = "TestUser",
                        },
                    RandomProvider = randomMock.Object
                };

            var result = handler.Execute(message, context);

            result.IsHandled.Should().BeFalse();
        }
    }
}
