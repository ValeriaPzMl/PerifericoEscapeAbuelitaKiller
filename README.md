# Periférico: Escape Abuelita Killer

A 2D top-down **endless runner / vehicular combat** game built in Unity.

## Premise

You're a truck driver on Guadalajara's *Periférico* ring road, hauling a
truckload of passengers. Traffic out there is aggressive — other drivers
ram you on purpose — so you fight back: shoot, ram, and destroy enemy cars
to protect your passengers and survive as long (and as far) as possible.

Your **health bar doubles as your passenger count**: every hit you take
costs passengers, and running out of them ends the run.

## Play it

A ready-to-run Windows build lives in the [`Executable/`](Executable/)
folder — just grab the folder and run `CamionSimulator.exe`, no Unity
install required, if you want to give it a quick try.

## Controls

| Input | Action |
|---|---|
| `W` / `↑` | Accelerate |
| `S` / `↓` | Reverse / brake |
| `A` / `D` (or arrows) | Steer left / right |
| Mouse movement | Aim your equipped weapon |
| Right click | Fire |
| `Esc` | Pause |

## Core gameplay loop

1. Drive forward (the world only ever needs to track the Y axis — the road
   scrolls under/around the truck via procedural chunk spawning).
2. Distance traveled is tracked continuously and drives **all** difficulty
   and content pacing (more lanes, tougher enemies, new weapons unlock as you
   go further).
3. Normal traffic, aggressive enemies, weapon pickups and alcoholic-drink
   power-ups spawn ahead of you on a rolling basis.
4. Colliding with traffic, being shot, or crashing into the road's edge
   barriers damages you (`PlayerPhysicsController.takeDamage`); ramming or
   shooting enemies damages them back.
5. Destroying an *armed* enemy makes it drop its weapon as a pickup you can
   drive over to equip.
6. The run ends when your passenger count bottoms out, or when the truck
   stays flipped over for more than 4 seconds (`tiempoMaxVolcado`).
7. On death, your best distance and enemy kill count are compared against
   your saved records and you can view unlocked achievement medals.

## Combat system

- **`PlayerTruckController` (`PlayerPhysicsController`)**: drives the
  Rigidbody2D-based car physics (acceleration, steering angle, angular
  velocity from a simplified bicycle model) and owns the player's health/
  damage/shield logic.
- **`PlayerWeapons` / `Weapon` (`WeaponDemo`)**: the player starts unarmed
  and equips whatever weapon prefab they drive over. Firing (right click)
  aims at the mouse's world position — `MasShoot()` biases the aim point
  upward based on current speed, so shots lead the target better the faster
  you're going. Each weapon fires a pooled `Projectile` toward the click
  point.
- **`Projectile` (`ProjectileDemo`)**: travels to its target point, spawns a
  pooled impact/"hit" effect on arrival, and applies damage to whatever
  `Traffic`- or `Player`-tagged collider it lands near (`OverlapCircleAll`),
  while ignoring the shooter itself.
- **`EnemyWeapon`**: enemy-mounted turrets that only fire when actually
  visible on camera (`GeometryUtility` frustum check) *and* either the
  player is within `distanciaAtaque` or the enemy was recently attacked
  (`EstaSiendoAtacado`, a 5s "aggro" window) — this keeps offscreen enemies
  cheap and non-threatening until you engage them.
- **`WeaponPickup`**: sits on the road, and despawns itself once it falls
  too far behind the player; picking it up swaps the player's current
  weapon category via `PlayerWeapons.EquipWeapon`.
- **Weapon set** (unlocked over distance, see Procedural Generation below):
  pineapple catapult, T-shirt launcher, nail gun, confetti bazooka, ice
  cream catapult, energy cannon, bazooka, atomic bomb — each with its own
  prefab, projectile, and pooled impact VFX.

## Traffic & enemy AI (`TrafficCar`)

- Regular traffic drives straight ahead at a fixed speed and only reacts to
  collisions.
- A per-car `queTanAgresivo` (0–1) chance decides at spawn time whether a
  car becomes **aggressive**: aggressive cars actively drift sideways to
  match the player's lane (`SeguirLineaXAgresivo`) to force a collision.
- Cars deal and take damage on collision (with other traffic, with the
  player, or with the road's edge walls, which destroy them outright).
- Cars that fall too far behind the player (`distanciaMaxima`, checked every
  0.2s to save CPU) are despawned automatically rather than driving forever.
- Enemy-tagged cars (`enemigo == true`) drop a weapon pickup on death.
- Visibility (`OnBecameVisible`/`Invisible`) gates the lateral aggressive
  movement so offscreen cars don't do unnecessary work.

## Power-ups: the drinks

Power-ups spawn as bottles on the road and apply temporary buffs on pickup,
handled by `PowerUpManager`. Effects that scale damage stack multiplicatively
and are all reverted through `Invoke`-scheduled timers:

| Drink | Effect | Duration |
|---|---|---|
| Perla | x3 weapon damage | 30s |
| Tequila | x50 weapon damage | 5s |
| Mezcal | x10 weapon damage | 15s |
| Absinthe | x2 weapon damage | 60s |
| Michelada (Miche) | Full heal | instant |
| Whisky | Full damage immunity (shield) | 15s |
| Wine | 50% damage reduction | 30s |
| Pulque | +20 HP every 2s (regen) | 30s |

## Procedural generation

Three systems work together to build the level in front of the player as
they drive, and dismantle it behind them:

- **`CarreteraSpawner`** (road): keeps a rolling window of ~4 road chunks
  ahead of the player, pulling and returning them from the pool as the
  player advances. Lane-count changes (3 → 4 → 5 lanes) are handled by
  spawning a dedicated "transition" chunk prefab (`cambio34`, `cambio45`)
  instead of just swapping prefabs outright, so the road widens visually
  instead of popping.
- **`UnifiedSpawner`** (content): on a timer (`tiempoSpawn`), picks a lane
  ahead of the player and rolls a random outcome — power-up, enemy, or
  regular traffic — based on configurable probabilities
  (`probabilidadPowerUp`, `probabilidadEnemy`). Each spawn point is checked
  with `Physics2D.OverlapBox` first to avoid overlapping an existing car.
  Total live traffic is capped (`maxCoches`) via a static `TrafficCounter`
  that every `TrafficCar` increments/decrements on spawn/despawn. This same
  spawner also places weapon pickups on demand (`SpawnArma`).
- **`DificultadManager`** (pacing): watches distance traveled
  (`PositionManager.differenceX`) and drives content unlocks at fixed
  distance thresholds — widening the road to 4 lanes at 1 km and 5 lanes at
  2 km, and, roughly every 0.1–0.3 km, both raising the enemy difficulty
  tier (`UnifiedSpawner.SubirDificultad`, which widens the pool of enemy
  types that can spawn) and spawning the next weapon pickup in the
  progression (pineapple → shirts → nails → confetti → ice cream → energy →
  bazooka → atomic bomb).

Together this means the run always feels the same at km 0 but keeps ramping
in both spatial complexity (more lanes) and combat intensity (tougher
enemies, stronger weapons) purely as a function of how far the player has
survived — no hand-authored level layout.

## Object pooling

Because the game is constantly creating and destroying road chunks, traffic
cars, enemies, projectiles, impact VFX, weapon pickups and power-ups, almost
nothing is actually `Instantiate`d/`Destroy`ed at runtime — everything goes
through a shared pooling layer:

- **`IPooledObject`**: `OnSpawn()` / `OnDespawn()` lifecycle hooks, used
  instead of `Awake`/`OnDestroy` to reset state (health, animator, particle
  timers, audio) every time an object is reused.
- **`ObjectPool`**: a single named pool (e.g. `carro3`, `bazooka/Prefab`,
  `Sandia/proyectil`) backed by a `Queue<GameObject>`. It pre-warms
  `initialAmount` instances at startup, can optionally `expand` by
  instantiating more on demand, and carefully restores each object's
  original scale/parent on both spawn and return so re-parenting objects
  (e.g. a "hit" splat sticking to the car it hit) never leaves them
  stretched or shrunk.
- **`PoolCategory`**: groups related `ObjectPool`s under one name (e.g.
  `"carretera"`, `"PowerUps"`, `"enemigos"`, or one category per weapon
  type), each pool keyed by a sub-name (`"Prefab"`, `"proyectil"`, `"hit"`,
  `"Taker"`).
- **`PoolManager`**: a singleton that owns every category and exposes
  `GetFromPool(category, poolName)` / `ReturnToPool(category, poolName, obj)`
  as the single entry point every other script uses instead of touching
  `Instantiate`/`Destroy` directly.

This two-level `category → pool name` lookup is what lets one generic
spawner (`UnifiedSpawner`) and generic gameplay scripts (`TrafficCar`,
`ProjectileDemo`, `Hiteados`) handle dozens of different prefab types
(8 traffic cars, 8 enemy types, 8 power-ups, 8+ weapons each with their own
projectile/impact/pickup prefab) without any type-specific spawning code.

## Progression, stats & achievements

- **`PositionManager`** converts the truck's world-space Y movement into a
  "km traveled" value, which is the single source of truth every other
  system (difficulty, road widening, weapon unlocks, UI) reads from.
- **`DisplayData`** drives the in-run HUD: distance, passenger count
  (`vida / 20`), cars destroyed, and the temporary power-up indicators
  (damage multiplier badge, healing pop-up, shield opacity).
- **`GameOverController`** freezes the game, plays the death jingle, then
  hands the run's final distance/kills to `StatsManager`.
- **`StatsManager`** is a persistent singleton (`DontDestroyOnLoad`) that
  keeps the player's best distance and best kill count in `PlayerPrefs`
  across sessions, and flags whether the just-finished run beat a record.
- **`EndStatsUI`** / **`AchievementsWindowController`** present the run
  summary and unlock bronze/silver/gold medals once distance and kill-count
  thresholds are crossed.

## Project structure

```
Assets/Scrips/
├── drivers/        # PlayerTruckController (player physics/health), TrafficCar (AI)
├── weapons/         # Player & enemy weapons, projectiles, impact VFX, pickups, mouse aiming
├── power ups/        # Drink power-ups and their buff/debuff effects
├── pooling/           # Generic object pooling system (pool / category / manager)
├── gameplay gen/       # Road + content procedural spawners, difficulty pacing, camera, distance tracking
├── menus/               # HUD, pause, game over, stats persistence, achievements
└── no longer in use/     # Deprecated/legacy spawners, superseded by UnifiedSpawner
```

> The `no longer in use/` folder (old per-type spawners for traffic,
> enemies and power-ups, plus an early music player) is kept for reference
> but is not part of the current gameplay — everything it did is now
> handled by `UnifiedSpawner`.

