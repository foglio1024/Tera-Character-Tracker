// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Tera.Game.Messages
{
    public class SpawnMeServerMessage: ParsedMessage
    {
        public EntityId Id { get; private set; }

        public SpawnMeServerMessage(TeraMessageReader reader)
            : base(reader)
        {
            Id = reader.ReadEntityId();
        }
    }
}
