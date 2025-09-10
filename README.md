# PEC2-3D-FPS

## �C�mo usar?

Abrir el projecto dentro del directorio PEC2-3D-FPS, utilizando Unity 2021.3.19f.

Controles por defecto:
 - Jugador 1:
     - Movimiento: Teclas WASD
     - Correr: Sostener Shift mientras se mueve
     - Saltar: Barra Espaciadora
     - Disparar: Click Izquierdo
     - Recargar: Tecla R
     - Cambiar Arma: Tecla Tab

## Estructura L�gica del Proyecto

### Gestion de Partida

Se definen una serie de Clases para repartir las responsabilidades dentro del flujo de juego, comportamiento del jugador, armas, enemigos, comportamientos, objetos y plataformas.


- **GameManager**: El GameManager ser� responsable tanto del control del flujo del juego (GameLoop), como de la instanciaci�n de los vehiculos escogidos, manteniendo la espera cuando esta el conteo regresivo, habilitando la carrera mientras el jugador no haya completado 3 vueltas y finalmente controlar la finalizaci�n de la misma y mostrar los resultados. Tambien tendra en su conocimiento los waypoints y atajos existentes en la pista, los cuales le ser�n pasados a cada corredor para procesar su avance en la carrera.

- **PlayerShooter**: Este script ser� la referencia del jugador de FPS y que tendr� incorporado el FirstPersonController, as� como sus informaci�n dentro de la partida (Salud, Escudo, Inventario) y adjuntas, las armas que puede utilizar con su respectivo cargador y munici�n, adem�s de los clips de audio para diferentes eventos dentro del juego. Dentro del PlayerShooter se definen los comportamientos que el jugador puede realizar dentro del juego (Moverse, Disparar, Saltar, Correr, Recargar, Cambiar armas) basado en los inputs b�sicos de teclado mencionados anteriormente. 
El Player Shooter tambien tiene la logica que procesa cuando esta siendo atacado por enemigos y c�mo se reduce su vida y escudo al recibir da�os. 
Finalmente, para hacer que el HUD sea reactivo a los cambios que sufren los valores del jugador, se definen una serie de Actions a los que el UIHandler estar� suscrito para detectar cambios, de tal manera que cada que cambian datos de Salud, Escudo, Munici�n u objetos encontrados, estas acciones ser�n invocadas para notificar estos cambios. 

- **Weapon**: La definici�n extensible para cualquier arma, con su metodo Shoot que usar� el raycast para efectuar disparos y producir da�os a los enemigos, siempre y cuando el arma tenga el suficiente alcance para llegar a estos, as� como activar ciertos interruptores que implementen el script de PlatformSwitch. Para producir comportamientos que distingan un arma de otra, se definieron elementos como capacidad del cargador, munici�n inicial, munici�n m�xima, tiempo de recarga, da�o, cadencia de fuego y si el arma es autom�tica o no. 

- **EnemyAI**: Define el comportamiento de la inteligencia artificial de los enemigos. A diferencia del script original, este hace uso de los Delegates de C#, con lo cual cada una de las fases del comportamiento del enemigo (Patrulla, Alerta y Ataque) son definidas dentro de corrutinas que ir�n cambiando seg�n corresponda cada escenario. Este script se complementa fuertemente con el NavMeshAgent, definiendo una serie de waypoints por los que el enemigo se mover� dentro de lo que el NavMesh del escenario le permita, as� como reaccionar cuando el enemigo pase cerca de su area de detecci�n y empezar a atacar cuando est� en su radio de visi�n, el cual se define mediante un raycast que espera detectar al jugador. Los enemigos tambien tienen elementos variables para darle un comportamiento caracter�stico, resaltando vida, velocidad, rango de detecci�n, rango de visi�n, da�o y cadencia de fuego. 

- **EnemyDropper**: Al asociarlo a objetos que son destruibles, como los enemigos, se le a�ade la referencia a cualquier prefab que implemente el script Consumible. Cuando se activa el OnDestroy, se va a spawnear un objeto consumible con un valor determinado. 

- **UIHelper**:  Como se mencion� en el PlayerShooter, el UIHelper va a tener las referencias a los objetos del Canvas que compongan el HUD del jugador, con el fin de poder otorgarle al mismo la informaci�n correcta y actualizada de su estado actual en terminos de Salud, Escudo, Cargador, Munici�n y objetos adquiridos. Por cada uno de los Actions definidos en el PlayerShooter, habr� un metodo void que reaccionar� a los cambios de los valores a los que est� suscrito.

- **Consumable** : Define todos los objetos que pueden ser consumidos por el jugador, clasificados a trav�s del enum ConsumableType, que contiene los tipos HEALTH, AMMO, SHIELD, KEY. El objeto que implemente este script debe de tener un collider en trigger para funcionar, ya que al entrar en contacto con el jugador, este aplicara los cambios segun corresponda (otorgandole mas vida, escudo, munici�n o llaves) para luego desaparecer al ser consumido. Los objetos Consumable tambi�n pueden ser "droppeados" por los enemigos al ser derrotados.

- **AutomaticDoors**: Las puertas se abren y se cierran al detectar la pisada del jugador cercano con un boxCollider en trigger. Adicionalmente, se condiciona la apertura de las puertas asignandole una llave determinada para esto. 

- **MovingPlatform**: Este script se usa para las plataforma m�viles y flotantes dentro del juego, dejando al jugador como hijo del las mismas mientras esta montado en ellas para que el desplazamiento tenga el comportamiento esperado. Adicionalmente se le a�ade una propiedad para habilitar o deshabilitar su movimiento. 

- **PlatformSwitch**: Este script se usa enlazar un MovingPlatform, con el fin de hacer que se habilite o se deshabilite su movimiento como si de un interruptor se trata. Aprovechando un comportamiento similar al de los EnemyIA, estos se activar�n al ser disparados.


## Video de Demostraci�n

[Video de YouTube aqu� <----](https://youtu.be/55hmvZNqoFs)


## Estructura de Escenas

1. Pantalla Principal 
2. Mapa Artico
3. Cr�ditos

## Cr�ditos

Banda Sonora
- Dam - GoldenEye 007 N64 - Rareware

Assets Importados
- Assets_Crate_Barrels
- Boxes and Pallets
- Easy Primitive People
- Extrem Sci-fi LITE
- iPoly3D
- MedicalBox
- MetnalDreamAssets
- ModernWeaponsPack
- PopupAsylum
- Simple Box Pack

Efectos de Sonido
- RPG Maker VX Ace Runtime Package.
- https://kronbits.itch.io/freesfx
- GoldenEye 007 N64 - Rareware 


