using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Text.RegularExpressions;

namespace CyberSecurityChatbot
{
    public class UserMemory
    {
        public string Name { get; set; }
        public List<string> Interests { get; set; } = new List<string>();
        public string PreferredTopic { get; set; }
        public DateTime LastInteraction { get; set; }
        public int InteractionCount { get; set; }
    }

    public class ResponseData
    {
        public List<string> Responses { get; set; } = new List<string>();
        public string[] Keywords { get; set; }
        public string Category { get; set; }
    }

    public enum UserSentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Confident
    }

    public class CyberSecurityChatbot
    {
        private static UserMemory userMemory = new UserMemory();
        private static Dictionary<string, ResponseData> knowledgeBase;
        private static Random random = new Random();
        private static int conversationDepth = 0;

        static void Main()
        {
            try
            {
                Console.Title = "Advanced Cybersecurity Awareness Chatbot v2.0";
                InitializeKnowledgeBase();
                
                PlayGreeting();
                DisplayASCIIArt();
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n🔐 Welcome to the Advanced Cybersecurity Awareness Chatbot!");
                Console.WriteLine("I'm here to help you stay safe online with personalized advice.");
                Console.ResetColor();
                
                InitializeUser();
                MainConversationLoop();
            }
            catch (Exception ex)
            {
                HandleError($"Critical error: {ex.Message}");
            }
        }

        static void InitializeKnowledgeBase()
        {
            knowledgeBase = new Dictionary<string, ResponseData>
            {
                ["password"] = new ResponseData
                {
                    Keywords = new[] { "password", "passwords", "passphrase", "authentication" },
                    Category = "Authentication",
                    Responses = new List<string>
                    {
                        "🔒 Create strong passwords with at least 12 characters including uppercase, lowercase, numbers, and symbols. Avoid using personal information like birthdays or names.",
                        "🛡️ Use a unique password for each account. Consider using a password manager like Bitwarden or LastPass to generate and store complex passwords securely.",
                        "⚡ Enable password requirements: minimum 12 characters, mix of character types, and avoid dictionary words. Change passwords immediately if you suspect a breach."
                    }
                },
                ["phishing"] = new ResponseData
                {
                    Keywords = new[] { "phishing", "phish", "scam", "fake", "email", "suspicious" },
                    Category = "Email Security",
                    Responses = new List<string>
                    {
                        "🎣 Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations. Always verify the sender's identity through official channels.",
                        "🚨 Look for red flags: urgent language, spelling errors, suspicious links, or requests for sensitive data. When in doubt, contact the organization directly using official contact information.",
                        "🔍 Hover over links to see the actual destination before clicking. Legitimate organizations won't ask for passwords or sensitive information via email."
                    }
                },
                ["2fa"] = new ResponseData
                {
                    Keywords = new[] { "2fa", "two-factor", "multi-factor", "authentication", "mfa", "authenticator" },
                    Category = "Authentication",
                    Responses = new List<string>
                    {
                        "🔐 Two-Factor Authentication adds crucial security by requiring a second verification step. Use authenticator apps like Google Authenticator or Authy instead of SMS when possible.",
                        "📱 Enable 2FA on all important accounts: email, banking, social media, and work accounts. It significantly reduces the risk of unauthorized access even if your password is compromised.",
                        "🛡️ Backup your 2FA codes safely. Many authenticator apps offer cloud backup or recovery codes. Store these securely in case you lose your device."
                    }
                },
                ["malware"] = new ResponseData
                {
                    Keywords = new[] { "malware", "virus", "trojan", "ransomware", "spyware", "adware" },
                    Category = "Malware Protection",
                    Responses = new List<string>
                    {
                        "🦠 Keep your antivirus software updated and run regular scans. Windows Defender is effective for most users, but consider additional protection for high-risk environments.",
                        "⬇️ Only download software from official sources and verified app stores. Avoid cracked software, suspicious email attachments, and files from untrusted websites.",
                        "🔄 Enable automatic updates for your operating system and applications. Many malware attacks exploit known vulnerabilities that patches have already fixed."
                    }
                },
                ["privacy"] = new ResponseData
                {
                    Keywords = new[] { "privacy", "data", "tracking", "cookies", "personal", "information" },
                    Category = "Privacy Protection",
                    Responses = new List<string>
                    {
                        "🕵️ Review privacy settings on social media platforms and limit what information you share publicly. Use privacy-focused browsers like Firefox or Brave.",
                        "🍪 Manage cookies and tracking: clear them regularly, use incognito/private browsing for sensitive activities, and consider browser extensions that block trackers.",
                        "📊 Be mindful of what data you share with apps and services. Read privacy policies for important services and opt out of data collection when possible."
                    }
                },
                ["wifi"] = new ResponseData
                {
                    Keywords = new[] { "wifi", "wireless", "network", "public", "hotspot", "router" },
                    Category = "Network Security",
                    Responses = new List<string>
                    {
                        "📶 Avoid using public Wi-Fi for sensitive activities like banking. If you must use it, connect through a VPN to encrypt your traffic.",
                        "🔒 Secure your home Wi-Fi with WPA3 encryption (or WPA2 if WPA3 isn't available). Use a strong network password and change the default router login credentials.",
                        "🌐 Consider using your phone's hotspot instead of public Wi-Fi. It's generally more secure than open networks in cafes, airports, or hotels."
                    }
                },
                ["social"] = new ResponseData
                {
                    Keywords = new[] { "social", "media", "facebook", "instagram", "twitter", "linkedin", "sharing" },
                    Category = "Social Media Security",
                    Responses = new List<string>
                    {
                        "📱 Limit personal information on social profiles. Avoid posting location data, full birth dates, or information that could be used for identity theft.",
                        "👥 Be selective about friend/connection requests. Scammers create fake profiles to gather information or spread malicious links.",
                        "🔒 Use privacy settings to control who can see your posts and personal information. Regularly review and update these settings as platforms change their policies."
                    }
                },
                ["backup"] = new ResponseData
                {
                    Keywords = new[] { "backup", "data", "recovery", "restore", "sync", "cloud" },
                    Category = "Data Protection",
                    Responses = new List<string>
                    {
                        "💾 Follow the 3-2-1 backup rule: 3 copies of important data, on 2 different media types, with 1 stored offsite. This protects against hardware failure, theft, and disasters.",
                        "☁️ Use reputable cloud services like Google Drive, OneDrive, or iCloud for automatic backups, but encrypt sensitive files before uploading them.",
                        "🔄 Test your backups regularly to ensure they work when needed. Set up automatic backups for critical data and verify they're completing successfully."
                    }
                }
            };
        }

        static void PlayGreeting()
        {
            try
            {
                // Simulate sound with console beeps since SoundPlayer requires actual audio files
                for (int i = 0; i < 3; i++)
                {
                    Console.Beep(800, 200);
                    Thread.Sleep(100);
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("🔊 Welcome sound would play here!");
                Console.ResetColor();
            }
        }

        static void DisplayASCIIArt()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
 ╔═══════════════════════════════════════════════════════════╗
 ║   ██████╗██╗   ██╗██████╗ ███████╗██████╗  ██████╗██╗  ██╗║
 ║  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██║  ██║║
 ║  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██║     ███████║║
 ║  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██║     ██╔══██║║
 ║  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║╚██████╗██║  ██║║
 ║   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝║
 ║                 🛡️ SECURITY AWARENESS CHATBOT 🛡️           ║
 ╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void InitializeUser()
        {
            userMemory.Name = GetValidInput("\n👤 Please enter your name: ");
            userMemory.LastInteraction = DateTime.Now;
            userMemory.InteractionCount = 0;
            
            Console.WriteLine($"\n🎉 Hello, {userMemory.Name}! I'm your personal cybersecurity assistant.");
            Console.WriteLine("💡 I can help with passwords, phishing, malware, privacy, and more!");
            Console.WriteLine("🧠 I'll remember our conversation to provide better assistance.");
        }

        static void MainConversationLoop()
        {
            Console.WriteLine("\n" + new string('═', 60));
            Console.WriteLine("🚀 Let's start! Ask me anything about cybersecurity.");
            Console.WriteLine("Type 'help' for suggestions or 'exit' to quit.");
            Console.WriteLine(new string('═', 60));

            while (true)
            {
                try
                {
                    string input = GetUserInput();
                    
                    if (input.ToLower() == "exit")
                    {
                        DisplayGoodbye();
                        break;
                    }
                    
                    if (input.ToLower() == "help")
                    {
                        DisplayHelp();
                        continue;
                    }

                    ProcessUserInput(input);
                    conversationDepth++;
                    userMemory.InteractionCount++;
                    userMemory.LastInteraction = DateTime.Now;
                }
                catch (Exception ex)
                {
                    HandleError($"Error processing input: {ex.Message}");
                }
            }
        }

        static string GetUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n{userMemory.Name}> ");
            Console.ResetColor();
            
            string input = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(input) ? "help" : input;
        }

        static void ProcessUserInput(string input)
        {
            // Detect sentiment
            UserSentiment sentiment = DetectSentiment(input);
            
            // Find matching keywords and responses
            var matchedTopics = FindMatchingTopics(input);
            
            if (matchedTopics.Any())
            {
                foreach (var topic in matchedTopics.Take(1)) // Focus on best match
                {
                    RememberUserInterest(topic.Category);
                    DeliverResponse(topic, sentiment, input);
                }
                
                // Suggest follow-up questions
                SuggestFollowUp(matchedTopics.First().Category);
            }
            else
            {
                HandleUnknownInput(input, sentiment);
            }
        }

        static UserSentiment DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();
            
            if (ContainsWords(lowerInput, new[] { "worried", "scared", "afraid", "concerned", "anxious" }))
                return UserSentiment.Worried;
            if (ContainsWords(lowerInput, new[] { "curious", "interesting", "learn", "know more", "tell me" }))
                return UserSentiment.Curious;
            if (ContainsWords(lowerInput, new[] { "frustrated", "annoying", "difficult", "hard", "confusing" }))
                return UserSentiment.Frustrated;
            if (ContainsWords(lowerInput, new[] { "confident", "sure", "understand", "got it", "clear" }))
                return UserSentiment.Confident;
                
            return UserSentiment.Neutral;
        }

        static bool ContainsWords(string text, string[] words)
        {
            return words.Any(word => text.Contains(word));
        }

        static List<ResponseData> FindMatchingTopics(string input)
        {
            string lowerInput = input.ToLower();
            var matches = new List<(ResponseData data, int score)>();

            foreach (var kvp in knowledgeBase)
            {
                int score = 0;
                foreach (string keyword in kvp.Value.Keywords)
                {
                    if (lowerInput.Contains(keyword.ToLower()))
                    {
                        score += keyword.Length; // Longer keywords get higher priority
                    }
                }
                
                if (score > 0)
                {
                    matches.Add((kvp.Value, score));
                }
            }

            return matches.OrderByDescending(m => m.score)
                         .Select(m => m.data)
                         .ToList();
        }

        static void DeliverResponse(ResponseData topic, UserSentiment sentiment, string originalInput)
        {
            // Add sentiment-based intro
            string sentimentIntro = GetSentimentIntro(sentiment);
            
            // Select random response from available options
            string response = topic.Responses[random.Next(topic.Responses.Count)];
            
            // Personalize based on memory
            string personalizedResponse = PersonalizeResponse(response, topic.Category);
            
            // Display the response with proper formatting
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🤖 Chatbot:");
            Console.ResetColor();
            
            if (!string.IsNullOrEmpty(sentimentIntro))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(sentimentIntro);
                Console.ResetColor();
            }
            
            Console.WriteLine(personalizedResponse);
        }

        static string GetSentimentIntro(UserSentiment sentiment)
        {
            return sentiment switch
            {
                UserSentiment.Worried => "I understand you're concerned about this. Let me help ease your worries with some practical advice:",
                UserSentiment.Curious => "Great question! I love when people want to learn more about cybersecurity:",
                UserSentiment.Frustrated => "I can sense this might be frustrating. Let me break it down in a simpler way:",
                UserSentiment.Confident => "You seem to have a good understanding! Here's some additional insight:",
                _ => ""
            };
        }

        static void RememberUserInterest(string category)
        {
            if (!userMemory.Interests.Contains(category))
            {
                userMemory.Interests.Add(category);
            }
            
            // Update preferred topic based on frequency
            var interestCounts = userMemory.Interests.GroupBy(i => i)
                                          .OrderByDescending(g => g.Count())
                                          .FirstOrDefault();
            
            if (interestCounts != null)
            {
                userMemory.PreferredTopic = interestCounts.Key;
            }
        }

        static string PersonalizeResponse(string response, string category)
        {
            // Add personalized touches based on user memory
            if (userMemory.Interests.Contains(category) && userMemory.InteractionCount > 1)
            {
                response = $"As someone interested in {category.ToLower()}, you might also want to know: " + response;
            }
            
            if (userMemory.InteractionCount > 5 && userMemory.PreferredTopic == category)
            {
                response += $"\n\n💡 Since you're particularly interested in {category.ToLower()}, I'd recommend diving deeper into advanced practices in this area.";
            }
            
            return response;
        }

        static void SuggestFollowUp(string category)
        {
            var suggestions = new Dictionary<string, string[]>
            {
                ["Authentication"] = new[] { "What about biometric authentication?", "How do I set up 2FA on specific apps?", "What if I lose my authenticator device?" },
                ["Email Security"] = new[] { "How can I report phishing attempts?", "What about business email compromise?", "How do I secure my email account?" },
                ["Malware Protection"] = new[] { "What should I do if I think I have malware?", "How do I safely remove suspicious software?", "What about mobile malware?" },
                ["Privacy Protection"] = new[] { "How do I delete my data from companies?", "What about VPN recommendations?", "How private is incognito mode really?" },
                ["Network Security"] = new[] { "How do I secure my home router?", "What about IoT device security?", "How can I detect network intrusions?" },
                ["Social Media Security"] = new[] { "How do I spot fake social media profiles?", "What information should I never share online?", "How do I secure my accounts after a breach?" },
                ["Data Protection"] = new[] { "What's the best cloud storage for security?", "How do I encrypt sensitive files?", "What about business data backup strategies?" }
            };

            if (suggestions.ContainsKey(category))
            {
                var categoryQuestions = suggestions[category];
                string suggestion = categoryQuestions[random.Next(categoryQuestions.Length)];
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n💭 You might also ask: \"{suggestion}\"");
                Console.ResetColor();
            }
        }

        static void HandleUnknownInput(string input, UserSentiment sentiment)
        {
            string[] fallbackResponses = {
                "I'm not sure about that specific topic, but I'd be happy to help with cybersecurity questions!",
                "That's outside my expertise area. Try asking about passwords, phishing, malware, or privacy.",
                "I didn't quite understand that. Can you rephrase your cybersecurity question?",
                "I'm designed to help with cybersecurity topics. What would you like to know about staying safe online?"
            };

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n🤖 Chatbot: {fallbackResponses[random.Next(fallbackResponses.Length)]}");
            Console.ResetColor();
            
            // Offer suggestions based on user's interests
            if (userMemory.Interests.Any())
            {
                Console.WriteLine($"\n💡 Based on your previous interests in {string.Join(", ", userMemory.Interests.Take(3).Select(i => i.ToLower()))}, you might want to explore these topics further.");
            }
        }

        static void DisplayHelp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n📚 CYBERSECURITY TOPICS I CAN HELP WITH:");
            Console.ResetColor();
            
            var helpTopics = new Dictionary<string, string>
            {
                ["🔒 Passwords"] = "Strong password creation, password managers, authentication",
                ["🎣 Phishing"] = "Email scams, suspicious links, social engineering attacks",
                ["🛡️ Two-Factor Auth"] = "2FA setup, authenticator apps, account security",
                ["🦠 Malware"] = "Virus protection, safe downloads, system security",
                ["🕵️ Privacy"] = "Data protection, online tracking, privacy settings",
                ["📶 Wi-Fi Security"] = "Safe networking, public Wi-Fi, router security",
                ["📱 Social Media"] = "Profile security, safe sharing, privacy controls",
                ["💾 Data Backup"] = "File protection, cloud storage, recovery planning"
            };

            foreach (var topic in helpTopics)
            {
                Console.WriteLine($"{topic.Key}: {topic.Value}");
            }
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n💬 Just type your question naturally, like:");
            Console.WriteLine("• \"How do I create a strong password?\"");
            Console.WriteLine("• \"What is phishing?\"");
            Console.WriteLine("• \"Help me with 2FA setup\"");
            Console.ResetColor();
        }

        static void DisplayGoodbye()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n🎯 SESSION SUMMARY for {userMemory.Name}:");
            Console.WriteLine($"• Questions asked: {userMemory.InteractionCount}");
            Console.WriteLine($"• Topics explored: {string.Join(", ", userMemory.Interests)}");
            if (!string.IsNullOrEmpty(userMemory.PreferredTopic))
            {
                Console.WriteLine($"• Main interest: {userMemory.PreferredTopic}");
            }
            
            Console.WriteLine("\n🔐 FINAL SECURITY REMINDERS:");
            Console.WriteLine("✅ Use unique, strong passwords with 2FA");
            Console.WriteLine("✅ Keep software and systems updated");
            Console.WriteLine("✅ Be cautious with emails and downloads");
            Console.WriteLine("✅ Backup important data regularly");
            Console.WriteLine("✅ Stay informed about new threats");
            
            Console.WriteLine($"\n👋 Thank you for using the Cybersecurity Chatbot, {userMemory.Name}!");
            Console.WriteLine("🛡️ Stay vigilant and secure online!");
            Console.ResetColor();
            
            // Play goodbye sound
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.Beep(600, 300);
                    Thread.Sleep(200);
                }
            }
            catch { }
        }

        static string GetValidInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
            } while (string.IsNullOrEmpty(input));
            
            return input;
        }

        static void HandleError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ {message}");
            Console.WriteLine("The chatbot will continue to function with basic features.");
            Console.ResetColor();
        }
    }
}