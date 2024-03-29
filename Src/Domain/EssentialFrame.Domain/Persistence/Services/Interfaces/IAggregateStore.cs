﻿using System.Text;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

public interface IAggregateStore
{
    bool Exists(string aggregateIdentifier);

    Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    AggregateDataModel Get(string aggregateIdentifier);

    Task<AggregateDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToke);

    void Save(AggregateDataModel aggregate);

    Task SaveAsync(AggregateDataModel aggregate, CancellationToken cancellationToken);

    void Box(string aggregateIdentifier, Encoding encoding);

    Task BoxAsync(string aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken);

    AggregateDataModel Unbox(string aggregateIdentifier, Encoding encoding);

    Task<AggregateDataModel> UnboxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
}