using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.Tests.Mocks;

/// <summary>
/// A mock implementation of the <see cref="IProcessRunner"/> interface.
/// Used for unit testing to simulate and verify the behavior of external process execution.
/// </summary>
public class MockProcessRunner : IProcessRunner {

  private readonly Mock<IProcessRunner> _mock = new();

  /// <summary>
  /// Sets up a mock behavior for the specified expression on the <see cref="IProcessRunner"/> interface.
  /// </summary>
  /// <param name="expression">An expression that represents a void member on <see cref="IProcessRunner"/> to set up.</param>
  /// <returns>An instance of <see cref="ISetup{TMock}"/> for configuring the mock behavior.</returns>
  public ISetup<IProcessRunner> Setup(Expression<Action<IProcessRunner>> expression) {
    return _mock.Setup(expression);
  }

  /// <summary>
  /// Sets up a mock behavior for the specified expression on the <see cref="IProcessRunner"/> interface.
  /// </summary>
  /// <typeparam name="TResult">The return type of the member being set up.</typeparam>
  /// <param name="expression">An expression that represents the member on <see cref="IProcessRunner"/> to set up.</param>
  /// <returns>An instance of <see cref="ISetup{TMock, TResult}"/> for configuring the mock behavior.</returns>
  public ISetup<IProcessRunner, TResult> Setup<TResult>(Expression<Func<IProcessRunner, TResult>> expression) {
    return _mock.Setup(expression);
  }

  /// <inheritdoc />
  public Task<int> RunProcess(string command, string[] arguments) {
    return _mock.Object.RunProcess(command, arguments);
  }
}