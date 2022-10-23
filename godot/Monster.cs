using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsHidingInTheDarkSpooky2DJam2022
{
    public class Monster
    {
        public string Name { get; set; }
        public string LocaleName { get; set; }
        public float EncounterRate { get; set; }
        public ActionResponse[] ActionResponses { get; set; }

        public static Monster[] Monsters
        {
            get
            {
                return new Monster[]
                {
                    new Monster
                    {
                        Name = "Human-Witch",
                        LocaleName = "Trick or treator",
                        EncounterRate = 0.20f,
                        ActionResponses = null
                    },
                    new Monster
                    {
                        Name = "Human-Ghost",
                        LocaleName = "Trick or treator",
                        EncounterRate = 0.20f,
                        ActionResponses = null
                    },
                    new Monster {
                        Name = "TongueSpider",
                        LocaleName = "Tongue Spider",
                        EncounterRate = 0.5f,
                        ActionResponses = new ActionResponse[]
                        {
                            new ActionResponse {
                                Name = "Attic",
                                ResponseText = "You don't have an attic. The tongue spider licks you before retching over and leaving.",
                                CourageModifier = -3
                            },
                            new ActionResponse {
                                Name = "Attack",
                                ResponseText = "You begin to engage the spider, but then, it scurries away.",
                                CourageModifier = 0
                            },
                            new ActionResponse {
                                Name = "A tick",
                                ResponseText = "What are you talking about?\nThis is a tongue spider, not a tongue tick. The tongue spider licks you before retching over and leaving.",
                                CourageModifier = -3
                            }
                        }
                    },
                    new Monster
                    {
                        Name = "Mask",
                        LocaleName = "Mask",
                        EncounterRate = 0.10f,
                        ActionResponses = new ActionResponse[]
                        {
                            new ActionResponse {
                                Name = "Wear",
                                ResponseText = "You attempt to put the mask on your body. The mask polietly declines and explains that they need to be somewhere else.",
                                CourageModifier = 0
                            },
                            new ActionResponse {
                                Name = "Where",
                                ResponseText = "They are right there! The mask seems a little offended that you don't see it and leaves.",
                                CourageModifier = -3
                            },
                            new ActionResponse {
                                Name = "Weak",
                                ResponseText = "You attempt to engage the mask in combat. You throw a punch, and the mask laughs at you. It seems satisfied.",
                                CourageModifier = -2
                            }
                        }
                    },
                    new Monster
                    {
                        Name = "Ghost",
                        LocaleName = "Ghost",
                        EncounterRate = 0.15f,
                        ActionResponses = new ActionResponse []
                        {
                            new ActionResponse {
                                Name = "Spook",
                                ResponseText = "You attempt to scare the spectre. The ghost seems offended and definitely doesn't want to be your friend anymore.",
                                CourageModifier = -2
                            },
                            new ActionResponse {
                                Name = "Sqeak",
                                ResponseText = "You squeak. The ghost seems convinced that you are a mouse and leaves you alone.",
                                CourageModifier = 0
                            },
                            new ActionResponse {
                                Name = "Speak",
                                ResponseText = "You attempt to have a conversation. The ghost begins to cry before they leave.",
                                CourageModifier = 0
                            }
                        }
                    },
                    new Monster
                    {
                        Name = "EyeCrab",
                        LocaleName = "Eye Monster",
                        EncounterRate = 0.20f,
                        ActionResponses = new ActionResponse []
                        {
                            new ActionResponse {
                                Name = "Peek",
                                ResponseText = "You look into the eye monster's eyes. They look back at you. They seem pleased by this.",
                                CourageModifier = 0
                            },
                            new ActionResponse {
                                Name = "Poke",
                                ResponseText = "You poke the eye monster's eyes, and they begin to cry. You wonder if maybe you were the monster all along.",
                                CourageModifier = -1
                            },
                            new ActionResponse {
                                Name = "Pass",
                                ResponseText = "You avoid eye contact with the eye monster. They seem offended and leave.",
                                CourageModifier = -2
                            }
                        }
                    }
                };
            }
        }
    }
}
