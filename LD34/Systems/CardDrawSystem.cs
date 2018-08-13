using Artemis;
using Artemis.System;
using LD34.Components;
using LD34.Game.Cards;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD34.Systems
{
    internal sealed class CardDrawSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;

        // Content.
        private readonly SpriteFont cardAttributeFont, cardTextFont;
        private readonly Texture2D cardHandleTop, cardHandleBottom, cardMinionFront, cardSpellFront, cardBack;

        public CardDrawSystem(SpriteBatch spriteBatch, ContentManager content)
            : base(Aspect.All(typeof(CardComponent), typeof(PositionComponent)))
        {
            this.spriteBatch = spriteBatch;

            // Load content.
            cardAttributeFont = content.Load<SpriteFont>("Fonts/card_attribute_font");
            cardTextFont = content.Load<SpriteFont>("Fonts/card_text_font");
            cardHandleTop = content.Load<Texture2D>("Textures/card_handle_top");
            cardHandleBottom = content.Load<Texture2D>("Textures/card_handle_bottom");
            cardMinionFront = content.Load<Texture2D>("Textures/card_minion_front");
            cardSpellFront = content.Load<Texture2D>("Textures/card_spell_front");
            cardBack = content.Load<Texture2D>("Textures/card_back");
        }

        public override void Process(Entity entity)
        {
            CardComponent cardComponent = entity.GetComponent<CardComponent>();
            PositionComponent positionComponent = entity.GetComponent<PositionComponent>();

            Vector2 position = positionComponent.Position + cardComponent.Offset;
            if (cardComponent.FaceUp)
            {
                spriteBatch.Draw(cardHandleTop, position, Color.White * cardComponent.Opacity);
                DrawCentredString(cardTextFont, 
                    cardComponent.Card.Name, 
                    position + new Vector2(33f, 7f), 
                    (cardComponent.Growing ? Color.DarkGreen : (cardComponent.Held ? Color.DarkRed : Color.Black)) * cardComponent.Opacity);

                position += new Vector2(0f, cardHandleTop.Height);
                float height = cardMinionFront.Height * cardComponent.Separation;
                float textOpacity = ((cardComponent.Separation - 0.975f) / 0.025f) * cardComponent.Opacity;
                if (cardComponent.Card is MinionCard)
                {
                    MinionCard card = cardComponent.Card as MinionCard;
                    spriteBatch.Draw(cardMinionFront,
                        position,
                        new Rectangle(0,
                            (int)(cardMinionFront.Height * 0.5f - height * 0.5f),
                            cardMinionFront.Width,
                            (int)Math.Ceiling(height)),
                        Color.White * cardComponent.Opacity);

                    if (cardComponent.Separation >= 0.975f)
                    {
                        int maxHealthModifier = card.MaxHealthModifier;
                        DrawCentredString(cardAttributeFont, 
                            card.Health.ToString(), 
                            position + new Vector2(53f, 6f),
                            (card.Health < card.MaxHealth ? Color.DarkRed : Color.Black) * textOpacity);
                        if (maxHealthModifier != 0)
                            spriteBatch.DrawString(cardAttributeFont,
                                (maxHealthModifier > 0 ? "+" : "") + maxHealthModifier.ToString(),
                                position + new Vector2(60f, -2f),
                                (maxHealthModifier > 0 ? Color.Green : Color.Red) * textOpacity);

                        int initiativeModifier = card.InitiativeModifier;
                        DrawCentredString(cardAttributeFont, 
                            card.Initiative.ToString(), 
                            position + new Vector2(11f, 64f),
                            Color.Black * textOpacity);
                        if (initiativeModifier != 0)
                            spriteBatch.DrawString(cardAttributeFont,
                                (initiativeModifier > 0 ? "+" : "") + initiativeModifier.ToString(),
                                position + new Vector2(-cardAttributeFont.MeasureString((initiativeModifier > 0 ? "+" : "") + initiativeModifier.ToString()).X + 4f, 56f),
                                (initiativeModifier > 0 ? Color.Green : Color.Red) * textOpacity);

                        int attackModifier = card.AttackModifier;
                        DrawCentredString(cardAttributeFont, 
                            card.Attack.ToString(), 
                            position + new Vector2(53f, 64f),
                            Color.Black * textOpacity);
                        if (attackModifier != 0)
                            spriteBatch.DrawString(cardAttributeFont,
                                (attackModifier > 0 ? "+" : "") + attackModifier.ToString(),
                                position + new Vector2(60f, 56f),
                                (attackModifier > 0 ? Color.Green : Color.Red) * textOpacity);
                    }
                }
                else if (cardComponent.Card is SpellCard)
                {
                    SpellCard card = cardComponent.Card as SpellCard;
                    spriteBatch.Draw(cardSpellFront,
                        position,
                        new Rectangle(0,
                            (int)(cardSpellFront.Height * 0.5f - height * 0.5f),
                            cardSpellFront.Width,
                            (int)Math.Ceiling(height)),
                        Color.White * cardComponent.Opacity);
                }

                if (cardComponent.Separation >= 0.975f)
                {
                    int costModifier = cardComponent.Card.CostModifier;
                    DrawCentredString(cardAttributeFont,
                        cardComponent.Card.Cost.ToString(),
                        position + new Vector2(11f, 6f),
                        Color.Black * textOpacity);
                    if (costModifier != 0)
                        spriteBatch.DrawString(cardAttributeFont,
                            (costModifier > 0 ? "+" : "") + costModifier.ToString(),
                            position + new Vector2(-cardAttributeFont.MeasureString((costModifier > 0 ? "+" : "") + costModifier.ToString()).X + 4f, -2f),
                            (costModifier > 0 ? Color.Red : Color.Green) * textOpacity);

                    string descriptionSample = "";
                    for (int i = 0; i < cardComponent.Card.Description.Count; i++)
                        descriptionSample += "T" + (i < cardComponent.Card.Description.Count - 1 ? "\n" : "");
                    float descriptionHeight = cardTextFont.MeasureString(descriptionSample).Y;
                    for (int i = 0; i < cardComponent.Card.Description.Count; i++)
                        DrawCentredString(cardTextFont,
                            cardComponent.Card.Description[i],
                            position + new Vector2(32f, 35f - (descriptionHeight / 2f) + (descriptionHeight / cardComponent.Card.Description.Count) * (i + 0.5f)),
                            Color.Black * ((cardComponent.Separation - 0.975f) / 0.025f) * cardComponent.Opacity);
                }

                position += new Vector2(0f, height);
                spriteBatch.Draw(cardHandleBottom, position, Color.White * cardComponent.Opacity);
                DrawCentredString(cardTextFont, 
                    cardComponent.Card.Tag, 
                    position + new Vector2(33f, 7f), 
                    (cardComponent.Growing ? Color.DarkGreen : (cardComponent.Held ? Color.DarkRed : Color.Black)) * cardComponent.Opacity);
            }
            else
                spriteBatch.Draw(cardBack, position, Color.White * cardComponent.Opacity);
        }

        private void DrawCentredString(SpriteFont spriteFont, string text, Vector2 centre, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, centre - (spriteFont.MeasureString(text) / 2f), color);
        }
    }
}
