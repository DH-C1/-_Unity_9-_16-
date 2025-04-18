using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SpartaRPG_V
{
    internal class Start_char
    {
        static bool wantDone = false;       //      트리거

        // 무한을 주기 위해
        static void Main(string[] args)
        {
            ShowStartScreen();
            while(!wantDone)        //      wantDone이 true가 될때까지 무한히 반복됨 <트리거 키>
            {
                
            }
        }

        // 인벤토리 오픈 및 아이템 소모, 장착
        static void ShowInventory(Character player)
        {
            Console.Clear();
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("인벤토리에 아이템이 없습니다.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("%+=== 인벤토리 ===+%");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                var item = player.Inventory[i];     //      var 을 적용해 int 값의 아이템이 오거나 다른 값의 아이템을 받을 수 있다.

                string equippedMark = "";       //      장착 여부 표시 텍스트
                
                if (item.Type == ItemType.Weapon && player.EquippedWeapon == item)
                {
                    equippedMark = " [E]";
                }
                else if (item.Type == ItemType.Armor && player.EquippedArmor == item)
                {
                    equippedMark = " [yEe]";
                }

                Console.WriteLine($"{i + 1}. {item.Name} ({item.Type}){equippedMark} - {item.Description}");
            }

            Console.WriteLine("0. 닫기");
            Console.Write("쓸 물건 번호는: ");
            string? input = Console.ReadLine();

            if(int.TryParse(input, out int index) && index > 0 && index <= player.Inventory.Count)      //      ??
            {
                Item selectedItem = player.Inventory[index - 1];
                player.Inventory[index - 1].Use(player);
                Console.ReadKey();

                if(selectedItem.Type == ItemType.Consumable)
                {
                    player.Inventory.RemoveAt(index - 1);
                    Console.WriteLine($"'{selectedItem.Name}'은 사라졌다.");
                }
                else if(selectedItem.Type == ItemType.MpConsumable)
                {
                    player.Inventory.RemoveAt(index - 1);
                    Console.WriteLine($"'{selectedItem.Name}'은 사라졌다.");
                }
                else if (selectedItem.Type == ItemType.Weapon)
                {
                    if (player.EquippedWeapon == selectedItem)
                    {
                        player.Atk -= selectedItem.Power;
                        player.EquippedWeapon = null;
                        Console.WriteLine($"'{selectedItem.Name}'을 해제했다.");
                    }
                    else
                    {
                        if (player.EquippedWeapon != null)
                        {
                            player.Atk -= player.EquippedWeapon.Power;
                            Console.WriteLine($"기존의 '{player.EquippedWeapon.Name}'을 해제했다.");
                        }

                        player.EquippedWeapon = selectedItem;
                        player.Atk += selectedItem.Power;
                        Console.WriteLine($"'{selectedItem.Name}'을 장착했다.");
                    }
                }
                else if (selectedItem.Type == ItemType.Armor)
                {
                    if (player.EquippedArmor == selectedItem)
                    {
                        player.Def -= selectedItem.Power;
                        player.EquippedArmor = null;
                        Console.WriteLine($"'{selectedItem.Name}'을 해제했다.");
                    }
                    else
                    {
                        if (player.EquippedArmor != null)
                        {
                            player.EquippedArmor.Power -= player.EquippedArmor.Power;
                            Console.WriteLine($"기존의 '{player.EquippedArmor.Name}'을 해제했다.");
                        }

                        player.EquippedArmor = selectedItem;
                        player.Def += selectedItem.Power;
                        Console.WriteLine($"'{selectedItem.Name}'을 장착했다.");
                    }
                }
                    Console.WriteLine("\n진행하려면 아무 키나 누르시오.");
                Console.ReadKey();
            }
            else if (input == "0")
            {
                return;
            }
            else
            {
                Console.WriteLine("다른건 없다.");
                Thread.Sleep(1000);
            }
        }

        // 일시 정지 메뉴
        static void ShowMainMenu(Character player)
        {
            Console.Clear();
            Console.WriteLine("%+=== [일시중지] ===+%");
            Console.WriteLine("1. 인벤토리");
            Console.WriteLine("2. 캐릭터 정보");
            Console.WriteLine("3. 게임 저장");
            Console.WriteLine("4. 게임 종료");
            Console.WriteLine("5. 메뉴 닫기");
            Console.WriteLine("\n선택: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ShowInventory(player);      //      인벤토리 호출
                    break;
                case "2":
                    ShowCharacterInfo(player);      //      캐릭터 정보 호출
                    break;
                case "3":
                    Console.WriteLine("당신은 일지에 여정을 적었다.");
                    Thread.Sleep(1000);
                    SavePlayerData(player);
                    break;
                case "4":
                    Console.WriteLine("여정을 중단 후 쉬기로한다... 게임 종료.");
                    Environment.Exit(0);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("이 외에 뭐가 있다고 생각하는거지?");
                    break;
            }
            Console.ReadKey();
            ShowMainMenu(player);       //      메뉴 반복
        }

        //시작 메뉴 구성
        static void ShowStartScreen()
        {
            Console.Clear();
            Console.WriteLine("%+=====================+%");
            Console.WriteLine("       Sparta RPG_V      ");
            Console.WriteLine("%+=====================+%\n");
            Console.WriteLine("1. 새 게임 시작");
            Console.WriteLine("2. 불러오기");
            Console.WriteLine("3. 게임 종료\n");

            Console.WriteLine("Select: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    StartNewGame();
                    break;
                case "2":
                    LoadGame();
                    break;
                case "3":
                    EndGame();
                    break;
                default:
                    Console.WriteLine("옮지 않은 행동이다. 다시 생각해라.");
                    Console.ReadKey();
                    ShowStartScreen();
                    break;
            }
        }
        
        // 게임 첫 실행 시
        static void StartNewGame()
        {
            Console.WriteLine($"\n 누구도 모르는 한 용기있는 자의 여정이 시작됬다...");
            Character player = CreateCharacter();

            // 스토리
            ShowStory(
                "...오랜 전쟁 끝에 황폐해져버린 땅에 목적을 가지고 발을 딛는 자가 있었다.",
                "\n 그 자들은 \"어리석은자\" 혹은 \"방황하는 자\" 라고 불리웠다.",
                "\n 그 자들중 한 명이 당신이다.",
                "\n 당신의 목적은 무엇이고 여정의 일기는 어떻게 써내려갈 것인가?",
                "\n .......",
                "\n 그리고 당신의 여정은 시작되었다.");

            ChoosePath(player);    //    첫 탄생 후 길 선택
        }

        static void LoadGame()
        {
            Console.WriteLine("\n여정을 회고 해본다...");
        }

        static void EndGame()
        {
            Console.WriteLine("\n여정을 중단 후 쉬기로한다... 게임 종료.");
            Environment.Exit(0);
        }

        // 스토리 대사 연출
        static void ShowStory(params string[] lines)
        {
            foreach (string line in lines)
            {
                Console.Write(line);
                Thread.Sleep(1500);     //      스토리 대사가 1.5초마다 나오도록 연출
            }

            Console.WriteLine("\n[진행하려면 아무 키나 누르시오]");
            Console.ReadKey();
        }



        // 캐릭터
        class Character
        {
            public string? Name; //      이름
            public string? Job;  //      직업
            public int MaxHP;   //      최대 체력
            public int MaxMP;   //      최대 마력
            public int HP;      //      체력
            public int MP;      //      마력
            public int Gold;    //      재화
            public int Lv;      //      레벨
            public int Atk;     //      공격력
            public int Matk;    //      마력공격력
            public int Def;     //      방어력

            public List<Item> Inventory = new List<Item>();     //      인벤토리

            public Item EquippedWeapon = null;
            public Item EquippedArmor = null;
        }

        // 캐릭터 생성
        static Character CreateCharacter()
        {
            Console.Clear();
            Console.WriteLine("당신의 이름은 무엇이었습니까?");
            string? name = Console.ReadLine();

            Console.WriteLine("\n당신의 직업은 무엇이었습니까?");
            Console.WriteLine("1. 방랑 기사");
            Console.WriteLine("2. 은둔자");
            Console.WriteLine("3. 암살자");
            Console.WriteLine("4. 가진 것 없는 자");

            string? jobInput = Console.ReadLine();    //    지역 변수
            string job = "";
            int maxHP = 0;
            int maxMP = 0;
            int hp = 0;
            int mp = 0;
            int gold = 0;
            int lv = 0;
            int atk = 0;
            int matk = 0;
            int def = 0;

            switch (jobInput)
            {
                case "1":
                    job = "방랑 기사";
                    maxHP = 200;
                    maxMP = 20;
                    hp = 200;
                    mp = 20;
                    gold = 100;
                    lv = 5;
                    atk = 25;
                    matk = 3;
                    def = 20;
                    break;
                case "2":
                    job = "은둔자";
                    maxHP = 110;
                    maxMP = 110;
                    hp = 110;
                    mp = 110;
                    gold = 30;
                    lv = 4;
                    atk = 5;
                    matk = 35;
                    def = 5;
                    break;
                case "3":
                    job = "암살자";
                    maxHP = 140;
                    maxMP = 60;
                    hp = 140;
                    mp = 60;
                    gold = 100;
                    lv = 7;
                    atk = 30;
                    matk = 20;
                    def = 15;
                    break;
                case "4":
                    job = "가진 것 없는 자";
                    maxHP = 125;
                    maxMP = 10;
                    hp = 125;
                    mp = 10;
                    gold = 0;
                    lv = 0;
                    atk = 10;
                    matk = 10;
                    def = 5;
                    break;
                default:
                    Console.WriteLine("형태 없는 자");
                    job = "???";
                    maxHP = 999;
                    maxMP = 999;
                    hp = 999;
                    mp = 999;
                    gold = 999;
                    lv = 999;
                    atk = 999;
                    matk = 999;
                    def = 999;
                    break;
            }

            Character newChar = new Character
            {
                Name = name,
                Job = job,
                HP = hp,
                MP = mp,
                Gold = gold,
                Lv = lv,
                Atk = atk,
                Matk = matk,
                Def = def
            };

            Console.WriteLine($"\n{name} 의 직업은 {job} 이었다.");
            Console.WriteLine($"HP: {hp}, MP: {mp}, Gold: {gold}, Lv: {lv}, \nAtk: {atk}, Matk: {matk}, Def: {def}");
            Console.ReadKey();

            return newChar;
        }

        // 저장 매커니즘
        static void SavePlayerData(Character player)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(player.Name);
                writer.WriteLine(player.Job);
                writer.WriteLine(player.MaxHP);
                writer.WriteLine(player.MaxMP);
                writer.WriteLine(player.HP);
                writer.WriteLine(player.MP);
                writer.WriteLine(player.Gold);
                writer.WriteLine(player.Lv);
                writer.WriteLine(player.Atk);
                writer.WriteLine(player.Matk);
                writer.WriteLine(player.Def);

                writer.WriteLine(player.EquippedWeapon);
                writer.WriteLine(player.EquippedArmor);

                writer.WriteLine(player.Inventory.Count);

                foreach (var item in player.Inventory)
                {
                    writer.WriteLine(item.ToSaveString());
                }
            }

            Console.WriteLine("플레이어 데이터를 성공적으로 저장했습니다!");
        }




        // 캐릭터 정보
        static void ShowCharacterInfo(Character player)
        {
            Console.Clear();
            Console.WriteLine("%+=== [캐릭터 정보] ===+%");
            Console.WriteLine($"이름       : {player.Name}");
            Console.WriteLine($"직업       : {player.Job}");
            Console.WriteLine($"체력       : {player.HP}");
            Console.WriteLine($"마력       : {player.MP}");
            Console.WriteLine($"재화       : {player.Gold}");
            Console.WriteLine($"레벨       : {player.Lv}");
            Console.WriteLine($"공격력     : {player.Atk}");
            Console.WriteLine($"마력공격력 : {player.Matk}");
            Console.WriteLine($"방어력     : {player.Def}");
            Console.WriteLine("%+=====================+%");

            Console.WriteLine("돌아가려면 아무 키나 누르시오.");
            Console.ReadKey();
        }

        // 저장 경로
        public const string path = @"D:\Visual_Studio_Projects\Interface\SpartaRPG_V\Save.txt";


        // 아이템
        class Item
        {
            public string Name;     //      아이템 이름
            public string Description;      //      아이템 설명
            public ItemType Type;       //      아이템 종류
            public int HealAmount;      //      아이템 체력 회복량
            public int MpHealAmount;        //      아이템 마력 회복량
            public int Power;       //      스탯 강화
            public int Price;       //      아이템 가격
            public Item() { }       //      {}생성자를 추가하여  () {} 변수 둘 다 쓸 수 있게됨
            public Item(string name, string description, ItemType type, int healAmount, int mpHealAmount, int power, int price)
            {
                Name = name;
                Description = description;
                Type = type;
                HealAmount = healAmount;
                MpHealAmount = mpHealAmount;
                Power = power;
                Price = price;
            }

            public void Use(Character player)
            {
                switch (Type)
                {
                    case ItemType.Consumable:
                        player.HP += HealAmount;
                        Console.WriteLine($"{player.Name}은 {Name}을 사용해 체력이 {HealAmount}만큼 회복되었다.");
                        break;
                    case ItemType.MpConsumable:
                        player.MP += MpHealAmount;
                        Console.WriteLine($"{player.Name}은 {Name}을 사용해 마력이 {MpHealAmount}만큼 회복되었다.");
                        break;
                }
            }
            public string ToSaveString()        //      
            {
                return $"{Name}|{Description}|{(int)Type}|{HealAmount}|{MpHealAmount}|{Power}|{Price}";
            }
        }



        // 아이템 종류
        enum ItemType
        {
            Weapon,
            Armor,
            Consumable,
            MpConsumable
        }

        // 길 선택, 종류
        static void ChoosePath(Character player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("%+=== 당신이 향할 곳은? ===+%");
                Console.WriteLine("1. 메뉴");
                Console.WriteLine("2. 누추한 마을 (휴식, 상점)");
                Console.WriteLine("3. 그을리고 우거진 숲");
                Console.WriteLine("4. 폐허 도심");
                Console.WriteLine("%+=========================+%");
                Console.Write("\n 당신의 선택은: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowMainMenu(player);       //      메뉴 불러오기 <일시정지 메인메뉴>
                        break;
                    case "2":
                        EnterVillage(player);
                        break;
                    case "3":
                        EnterForest(player);
                        break;
                    case "4":
                        EnterRuins(player);
                        break;
                    default:
                        Console.WriteLine("어딜 가려는 거냐?");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }
        // 마을
        static void EnterVillage(Character player)
        {
            ShowStory(
                "누추한 외관의 마을엔 유일한 제정신의 사람들이 모여있다.",
                "\n 누추하지만서도 내부 마을 사람들은 서로를 의지하며 살아가고 있다."
                );

            while (true)
            {
                Console.Clear();
                Console.WriteLine("%+=== 누추한 마을 ===+%");
                Console.WriteLine("1. 메뉴");
                Console.WriteLine("2. 상점");
                Console.WriteLine("3. 숙박시설 (휴식)");
                Console.WriteLine("0. 떠난다");
                Console.Write("\n당신의 선택은: ");
                
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowMainMenu(player);       //      메뉴 불러오기 <일시정지 메인메뉴>
                        break;
                    case "2":
                        VisitShop(player);
                        break;
                    case "3":
                        VisitHotel(player);
                        break;
                    case "0":
                        Console.WriteLine("당신은 마을을 떠난다..");
                        Thread.Sleep(1000);
                        return;     //      마을에서 나갈 시 길 선택 메뉴 재출력
                    default:
                        Console.WriteLine("이봐, 뭘 하려고?");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        // 마을 상점
        static void VisitShop(Character player)
        {
            Console.Clear();
            Console.WriteLine("%+=== 마을 상점 ===+%");
            Console.WriteLine("1. 메뉴");
            Console.WriteLine("S. 물건 판매");
            Console.WriteLine("2. 체력 회복 포션 (40) - 35G");
            Console.WriteLine("3. 마력 회복 포션 (55) - 35G");
            Console.WriteLine("4. 가죽제 갑옷 (10) - 450G");
            Console.WriteLine("5. 먼지낀 무기 (8) - 500G");
            Console.WriteLine("6. 플레이트 갑옷 (17) - 750G");
            Console.WriteLine("7. 갓 단조된 무기 (13) - 800G");
            Console.WriteLine("8. 최상위 갑옷 (30) - 800G");
            Console.WriteLine("9. 장식된 무기 (35) - 1000G");
            Console.WriteLine("0. 나간다");
            Console.Write("\n당신의 선택은: ");
            string? input = Console.ReadLine();

            Item? item = null;

            switch (input)
            {
                case "1":
                    ShowMainMenu(player);       //      메뉴 불러오기 <일시정지 메인메뉴>
                    break;
                case "S":
                    SellItem(player);
                    break;
                case "2":
                    item = new Item("체력 회복 포션", "체력을 40 회복한다", ItemType.Consumable, 40, 0, 0, 35);
                    break;
                case "3":
                    item = new Item("마력 회복 포션", "마력을 55 회복한다", ItemType.MpConsumable, 0, 55, 0, 35);
                    break;
                case "4":
                    item = new Item("가죽제 갑옷", "색 바랜 가죽 갑옷이지만 누더기 보단 낫다.", ItemType.Armor, 0, 0, 10, 450);
                    break;
                case "5":
                    item = new Item("먼지낀 무기", "상점의 구석에 있던 무기다. ..이빠진 무기보단 낫다.", ItemType.Weapon, 0, 0, 8, 500);
                    break;
                case "6":
                    item = new Item("플레이트 갑옷", "단단한 플레이트로 덮인 갑옷이다.", ItemType.Armor, 0, 0, 17, 750);
                    break;
                case "7":
                    item = new Item("갓 단조된 무기", "브로드 소드의 형태를 띄고 있다. 갓 단조되어 예리하고 광이 난다.", ItemType.Weapon, 0, 0, 13, 800);
                    break;
                case "8":
                    item = new Item("최상위 갑옷", "전쟁이 발발하기 전, 호위기사가 입던 갑옷이다. 온갖 문양과 지금와서는 의미 없는 사치품이 달려있다.", ItemType.Armor, 0, 0, 30, 800);
                    break;
                case "9":
                    item = new Item("장식된 무기", "알 수 없는 문양이 새겨진 칼 등에 장식이 박힌 특대검 이다.", ItemType.Weapon, 0, 0, 35, 1000);
                    break;
                case "0":
                    Console.WriteLine("상점을 나간다...");
                    Thread.Sleep(1000);
                    return;
                default:
                    Console.WriteLine("이 외에 흥미로운 것은 없다.");
                    Thread.Sleep(1000);
                    break;
            }

            if (item != null)
            {
                if(player.Gold >= item.Price)
                {
                    player.Gold -= item.Price;
                    player.Inventory.Add(item);
                    Console.WriteLine($"{item.Name}을 {item.Price}에 구매했다.");
                }
                else
                {
                    Console.WriteLine("???: 돈이 없어?! 돈 없으면 죽어야지...");
                }

                Console.ReadKey();
            }
        }

        // 아이템 판매
        static void SellItem(Character player)
        {
            Console.Clear();
            Console.WriteLine("%+=== 팔 수 있는 아이템 ===+%");

            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("가방이 비어있다.");
                Thread.Sleep(1000);
                return;
            }

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                var item = player.Inventory[i];
                Console.WriteLine($"{i + 1}. {item.Name} - 판매가: {item.Price * 0.85}G");
            }

            Console.WriteLine("0. 닫기");
            Console.Write("\n판매할 물건 선택: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index <= player.Inventory.Count)
            {
                Item selectedItem = player.Inventory[index - 1];
                int sellPrice = (int)(selectedItem.Price * 0.85);       //      85퍼센트를 적용하기위해 0.85를 곱했고, int값을 유지하기 위해 묶어줬다.

                player.Gold += sellPrice;
                player.Inventory.RemoveAt(index - 1);

                Console.WriteLine($"{selectedItem.Name}을 {sellPrice}에 팔았다.");
                Thread.Sleep(1000);
            }
            else if (index != 0)
            {
                Console.WriteLine("잘못된 선택");
                Thread.Sleep(1000);
            }
        }

        // 숙박 시설
        static void VisitHotel(Character player)
        {
            Console.Clear();
            Console.WriteLine("%+=== 숙박 시설 ===+%");
            Console.WriteLine("1. 휴식 - 60G");
            Console.WriteLine("0. 나간다.");
            Console.Write("\n당신의 선택은: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    int cost = 60;

                    if (player.Gold >= cost)
                    {
                        player.Gold -= cost;
                        player.HP = player.MaxHP;
                        player.MP = player.MaxMP;
                    }
                    else
                    {
                        Console.WriteLine("자금이 부족하다.");
                        Thread.Sleep(1000);
                    }
                break;
                case "0":
                    Console.WriteLine("숙박 시설을 나간다...");
                    Thread.Sleep(1000);
                    return;
            }
        }

        // 그을린 숲
        static void EnterForest(Character player)
        {
            ShowStory(
                "전쟁으로 울창했던 숲은 온데간데 없다.",
                "\n 지금은 오직.. 그을리고 영원히 타고있는 숲 뿐이다.",
                "\n ................"
                );
            Thread.Sleep(1300);

            EnterDungeon(player, DungeonDifficulty.Easy, requiredDef: 4);   
        }

        // 폐허 도심
        static void EnterRuins(Character player)
        {
            ShowStory(
                "한때 황금빛으로 찬란했던 꿈같던 도시는 몰락했다.",
                "\n 지금은 오직.. 무너진 잔해와 사라지지 못한 절규만이 들린다.",
                "\n ................"
                );
            Thread.Sleep(1300);

            EnterDungeon(player, DungeonDifficulty.Hard, requiredDef: 34);
        }

        // 던전 난이도 종류
        enum DungeonDifficulty
        {
            Easy,
            Normal,
            Hard
        }

        // 던전 매커니즘
        static void EnterDungeon(Character player, DungeonDifficulty difficulty, int requiredDef)
        {
            Console.Clear();
            Console.WriteLine($"던전에 들어섰다.");      //      나중에 던전 명 적용할 것
            Thread.Sleep(1000);

            Random random = new Random();
            int failChance = 40;
            int minLoss = 20;
            int maxLoss = 35;

            bool success = false;

            if (player.Def < requiredDef)
            {
                Console.WriteLine("당신의 의지가 시험받을 것이다.");
                Thread.Sleep (2300);
                //      40퍼센트 확률로 실패
                if (random.Next(0, 100) < failChance)       //      해석해봐야겠다.
                {
                    Console.WriteLine("의지가 무너졌다.");
                    Thread.Sleep(1700);
                    Console.WriteLine("당신의 몸 만큼은 던전을 빠져나왔다.");
                    Console.ReadLine();
                    player.HP /= 2;
                    return;
                }
                else
                {
                    Console.WriteLine("위기 속에서도 당신은 의지를 굳혔다.");
                    Thread.Sleep(1500);
                    success = true;
                }
            }
            else
            {
                Console.WriteLine("여유로운 전투가 될 것 같다.");
                Thread.Sleep(1000);
                success = true;
            }

            if (success)
            {   // 체력 소모
                int defGap = requiredDef - player.Def;
                int hpLossMin = minLoss + defGap;
                int hpLossMax = maxLoss + defGap;

                if (hpLossMin < 0) hpLossMin = 0;
                if (hpLossMax < hpLossMin) hpLossMax = hpLossMin;

                int hpLoss = random.Next(hpLossMin, hpLossMax + 1);
                player.HP -= hpLoss;
                if (player.HP <  0) player.HP = 0;

                Console.WriteLine($"체력이 {hpLoss}만큼 닳아 {player.HP}/{player.MaxHP}가 되었다.");
                Thread.Sleep(1000);

                // 보상 계산
                int baseReward = difficulty switch
                {
                    DungeonDifficulty.Easy => 40,
                    DungeonDifficulty.Normal => 100,
                    DungeonDifficulty.Hard => 150,
                    _ => 0,     //      ?? 이건 뭐지
                };

                int bonusPercent = random.Next(player.Atk, player.Atk * 2 + 1);
                int totalReward = baseReward + (baseReward * bonusPercent / 100);

                player.Gold += totalReward;

                Console.WriteLine($"\n 생환했다.");
                Console.WriteLine($"전리품: {baseReward} G");
                Console.WriteLine($"전투력 기반 추가 전리품 ({bonusPercent}%): {totalReward - baseReward} G");
                Console.WriteLine($"총 얻은 전리품: {totalReward} G");
                Console.WriteLine($"현재 보유 자금: {player.Gold} G");
                Console.ReadLine();
            }
        }

        // 사망 체크
        static bool CheckIfDead(Character player)
        {
            if (player.HP <= 0)
            {
                Console.Clear();
                ShowCharacterInfo(player);
                return true;        //      죽었을 시
            }
            return false;       //      살았을 시
        }

        // 사망시 엔딩
        static void ShowDeathEnding(Character player)
        {
            Console.WriteLine("%+====== 당신의 여정은 끝이 났습니다.. ======+%");
            Console.WriteLine($" 레벨: {player.Lv}");
            Console.WriteLine($" 남"
        }

    }
}
