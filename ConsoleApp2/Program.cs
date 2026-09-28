using System;
using System.Collections.Generic;

namespace RpgPatternSystem
{
    public class GameSettings
    {
        private static GameSettings _instance;
        private static readonly object _lock = new object();
        public string GameName { get; set; } = "RPG SYSTEM";
        public string Difficulty { get; set; } = "Normal";
        public int MaxLevel { get; set; } = 100;
        public int SoundVolume { get; set; } = 80;
        private GameSettings() { }
        public static GameSettings Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameSettings();
                    }
                    return _instance;
                }
            }
        }
    }

    public interface IWeapon
    {
        string GetName();
        int GetDamage();
    }
    public class Sword : IWeapon
    {
        public string GetName() => "Sword";
        public int GetDamage() => 15;
    }
    public class Bow : IWeapon
    {
        public string GetName() => "Bow";
        public int GetDamage() => 12;
    }
    public class Staff : IWeapon
    {
        public string GetName() => "Magic Staff";
        public int GetDamage() => 10;
    }
    public abstract class WeaponDecorator : IWeapon
    {
        protected IWeapon _decoratee;
        protected WeaponDecorator(IWeapon weapon) => _decoratee = weapon;

        public virtual string GetName() => _decoratee.GetName();
        public virtual int GetDamage() => _decoratee.GetDamage();
    }
    public class FireDecorator : WeaponDecorator
    {
        public FireDecorator(IWeapon weapon) : base(weapon) { }
        public override string GetName() => "Fire " + _decoratee.GetName();
        public override int GetDamage() => _decoratee.GetDamage() + 5;
    }
    public class PoisonDecorator : WeaponDecorator
    {
        public PoisonDecorator(IWeapon weapon) : base(weapon) { }
        public override string GetName() => "Poison " + _decoratee.GetName();
        public override int GetDamage() => _decoratee.GetDamage() + 3;
    }
    public class CriticalDecorator : WeaponDecorator
    {
        public CriticalDecorator(IWeapon weapon) : base(weapon) { }
        public override string GetName() => "Critical " + _decoratee.GetName();
        public override int GetDamage() => _decoratee.GetDamage() * 2;
    }
    public interface IArmor
    {
        string GetName();
        int GetDefense();
    }
    public class HeavyArmor : IArmor
    {
        public string GetName() => "Heavy Armor";
        public int GetDefense() => 20;
    }
    public class LightArmor : IArmor
    {
        public string GetName() => "Light Armor";
        public int GetDefense() => 10;
    }
    public class Robe : IArmor
    {
        public string GetName() => "Mage Robe";
        public int GetDefense() => 5;
    }
    public interface IEquipmentFactory
    {
        IWeapon CreateWeapon();
        IArmor CreateArmor();
    }
    public class WarriorEquipmentFactory : IEquipmentFactory
    {
        public IWeapon CreateWeapon() => new Sword();
        public IArmor CreateArmor() => new HeavyArmor();
    }
    public class MageEquipmentFactory : IEquipmentFactory
    {
        public IWeapon CreateWeapon() => new Staff();
        public IArmor CreateArmor() => new Robe();
    }
    public class ArcherEquipmentFactory : IEquipmentFactory
    {
        public IWeapon CreateWeapon() => new Bow();
        public IArmor CreateArmor() => new LightArmor();
    }
    public interface IAttack
    {
        void Attack(string characterName);
    }
    public class PhysicalAttack : IAttack
    {
        public void Attack(string characterName) =>
            Console.WriteLine($"{characterName} наносит сокрушительный физический удар!");
    }
    public class MagicAttack : IAttack
    {
        public void Attack(string characterName) =>
            Console.WriteLine($"{characterName} творит таинственное заклинание!");
    }
    public class RangedAttack : IAttack
    {
        public void Attack(string characterName) =>
            Console.WriteLine($"{characterName} производит точный выстрел издалека!");
    }
    public class OldCombatSystem
    {
        public void MakeHit(string name, int power)
        {
            Console.WriteLine($"{name} наносит удар силой {power} через старую боевую систему!");
        }
    }
    public class OldCombatAdapter : IAttack
    {
        private readonly OldCombatSystem _oldSystem;
        private readonly int _power;

        public OldCombatAdapter(OldCombatSystem oldSystem, int power = 10)
        {
            _oldSystem = oldSystem;
            _power = power;
        }
        public void Attack(string characterName)
        {
            _oldSystem.MakeHit(characterName, _power);
        }
    }
    public interface IUnit
    {
        void Attack();
    }
    public class Character : IUnit
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public IWeapon Weapon { get; set; }
        public IArmor Armor { get; set; }
        public IAttack AttackStrategy { get; set; }
        public Character() { }
        protected Character(Character source)
        {
            this.Name = source.Name;
            this.ClassName = source.ClassName;
            this.Level = source.Level;
            this.Health = source.Health;
            this.Weapon = source.Weapon;
            this.Armor = source.Armor;
            this.AttackStrategy = source.AttackStrategy;
        }
        public Character Clone()
        {
            return new Character(this);
        }
        public void Attack()
        {
            if (AttackStrategy != null)
                AttackStrategy.Attack(Name);
            else
                Console.WriteLine($"{Name} встал в ступор и не знает как атаковать.");
        }
        public void ShowInfo()
        {
            Console.WriteLine($"Персонаж: {Name} | Класс: {ClassName} | Уровень: {Level} | HP: {Health} | Оружие: {Weapon?.GetName() ?? "Нет"} (Урон: {Weapon?.GetDamage() ?? 0}) | Броня: {Armor?.GetName() ?? "Нет"}");
        }
    }
    public interface ICharacterFactory
    {
        Character CreateCharacter();
    }
    public class WarriorFactory : ICharacterFactory
    {
        public Character CreateCharacter()
        {
            var warrior = new Character { ClassName = "Warrior", Health = 120, Level = 1 };
            warrior.AttackStrategy = new PhysicalAttack();
            return warrior;
        }
    }
    public class MageFactory : ICharacterFactory
    {
        public Character CreateCharacter()
        {
            var mage = new Character { ClassName = "Mage", Health = 80, Level = 1 };
            mage.AttackStrategy = new MagicAttack();
            return mage;
        }
    }
    public class ArcherFactory : ICharacterFactory
    {
        public Character CreateCharacter()
        {
            var archer = new Character { ClassName = "Archer", Health = 95, Level = 1 };
            archer.AttackStrategy = new RangedAttack();
            return archer;
        }
    }
    public class CharacterBuilder
    {
        private Character _character = new Character();

        public CharacterBuilder Reset()
        {
            _character = new Character();
            return this;
        }

        public CharacterBuilder SetName(string name)
        {
            _character.Name = name;
            return this;
        }
        public CharacterBuilder SetClass(string className)
        {
            _character.ClassName = className;
            return this;
        }
        public CharacterBuilder SetLevel(int level)
        {
            _character.Level = level;
            return this;
        }
        public CharacterBuilder SetHealth(int health)
        {
            _character.Health = health;
            return this;
        }
        public CharacterBuilder SetWeapon(IWeapon weapon)
        {
            _character.Weapon = weapon;
            return this;
        }
        public CharacterBuilder SetArmor(IArmor armor)
        {
            _character.Armor = armor;
            return this;
        }
        public CharacterBuilder SetAttack(IAttack attack)
        {
            _character.AttackStrategy = attack;
            return this;
        }
        public Character Build()
        {
            Character completedCharacter = _character;
            Reset();
            return completedCharacter;
        }
    }
    public class Squad : IUnit
    {
        public string SquadName { get; set; }
        private readonly List<IUnit> _units = new List<IUnit>();

        public Squad(string name) => SquadName = name;

        public void Add(IUnit unit) => _units.Add(unit);
        public void Remove(IUnit unit) => _units.Remove(unit);

        public void Attack()
        {
            Console.WriteLine($"\n Отряд [{SquadName}] идет в атаку");
            foreach (var unit in _units)
            {
                unit.Attack();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Шаг 1. Настройки (Singleton)");
            GameSettings settings1 = GameSettings.Instance;
            GameSettings settings2 = GameSettings.Instance;

            Console.WriteLine($"Название: {settings1.GameName}");
            Console.WriteLine($"Сложность: {settings1.Difficulty}");
            Console.WriteLine($"Максимальный уровень: {settings1.MaxLevel}");
            Console.WriteLine($"Проверка Singleton (один и тот же объект?): {settings1 == settings2}");
            Console.WriteLine("\nШаг 2. Создание базового шаблона персонажа через Factory Method");
            ICharacterFactory warriorFactory = new WarriorFactory(); Character rawWarrior = warriorFactory.CreateCharacter();
            Console.WriteLine($"Создан базовый шаблон класса: {rawWarrior.ClassName}");
            Console.WriteLine("\nШаг 3. Создание экипировки через Abstract Factory");
            IEquipmentFactory warriorEquipFactory = new WarriorEquipmentFactory();
            IWeapon sword = warriorEquipFactory.CreateWeapon();
            IArmor heavyArmor = warriorEquipFactory.CreateArmor();
            Console.WriteLine($"Создано совместимое снаряжение -> Оружие: {sword.GetName()}, Броня: {heavyArmor.GetName()}");
            Console.WriteLine("\nШаг 4. Пошаговая сборка через Builder");
            CharacterBuilder builder = new CharacterBuilder();
            Character rei = builder.SetName("Rei").SetClass("Warrior").SetLevel(10).SetHealth(150).SetWeapon(sword).SetArmor(heavyArmor).SetAttack(new PhysicalAttack()).Build(); rei.ShowInfo();
            Console.WriteLine("\nШаг 5. Модификация оружия через Decorator");
            IWeapon decoratedWeapon = rei.Weapon;
            decoratedWeapon = new FireDecorator(decoratedWeapon);
            Console.WriteLine($"Добавлен эффект: {decoratedWeapon.GetName()} (Текущий урон: {decoratedWeapon.GetDamage()})");
            decoratedWeapon = new PoisonDecorator(decoratedWeapon);
            Console.WriteLine($"Добавлен эффект: {decoratedWeapon.GetName()} (Текущий урон: {decoratedWeapon.GetDamage()})");
            decoratedWeapon = new CriticalDecorator(decoratedWeapon);
            Console.WriteLine($"Добавлен эффект: {decoratedWeapon.GetName()} (Итоговый урон: {decoratedWeapon.GetDamage()})");
            rei.Weapon = decoratedWeapon;
            Console.WriteLine("\nШаг 6. Использование Bridge");
            rei.Attack();
            rei.AttackStrategy = new MagicAttack();
            rei.Attack();
            Console.WriteLine("\nШаг 7. Подключение сторонней системы через Adapter");
            OldCombatSystem oldSystem = new OldCombatSystem();
            IAttack adaptedAttack = new OldCombatAdapter(oldSystem, power: 99);
            rei.AttackStrategy = adaptedAttack;
            rei.Attack();
            Console.WriteLine("\nШаг 8. Копирование персонажа через Prototype");
            Character tolikOriginal = builder.SetName("Толик").SetClass("Warrior").SetLevel(10).SetHealth(150).SetWeapon(new Sword()).SetArmor(new HeavyArmor()).SetAttack(new PhysicalAttack()).Build();
            Console.WriteLine("Оригинал до клонирования:"); tolikOriginal.ShowInfo(); Character andreyClone = tolikOriginal.Clone();
            andreyClone.Name = "Андрэй";
            andreyClone.Level = 20;
            Console.WriteLine("\nПосле изменения клона:");
            Console.Write("Оригинал (не должен измениться): ");
            tolikOriginal.ShowInfo(); Console.Write("Клон (должен иметь 20 уровень): ");
            andreyClone.ShowInfo();
            Console.WriteLine("\nШаг 9. Объединение в группы через Composite");
            ICharacterFactory archerFactory = new ArcherFactory();
            ICharacterFactory mageFactory = new MageFactory();
            Character freeWarrior = warriorFactory.CreateCharacter();
            freeWarrior.Name = "Одинокий Воин";
            Character freeMage = mageFactory.CreateCharacter();
            freeMage.Name = "Вольный Маг";
            Squad squad1 = new Squad("Squad 1");
            Character s1Archer = archerFactory.CreateCharacter();
            s1Archer.Name = "Лучник Сквада 1";
            Character s1Warrior = warriorFactory.CreateCharacter();
            s1Warrior.Name = "Мечник Сквада 1";
            Character s1Mage = mageFactory.CreateCharacter();
            s1Mage.Name = "Маг Сквада 1";
            squad1.Add(s1Archer);
            squad1.Add(s1Warrior);
            squad1.Add(s1Mage);
            Squad squad2 = new Squad("Squad 2");
            Character s2Warrior = warriorFactory.CreateCharacter();
            s2Warrior.Name = "Рыцарь Сквада 2";
            Character s2Archer = archerFactory.CreateCharacter();
            s2Archer.Name = "Снайпер Сквада 2";
            squad2.Add(s2Warrior);
            squad2.Add(s2Archer);
            Squad army = new Squad("Главная Армия");
            army.Add(freeWarrior); army.Add(freeMage);
            army.Add(squad1);
            army.Add(squad2);
            army.Attack(); Console.ReadLine();
        }
    }
}