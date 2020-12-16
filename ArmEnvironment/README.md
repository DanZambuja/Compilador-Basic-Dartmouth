Hello World (ARM Flavor)
========================

This is a test for making a "Hello World" app for the ARM architecture.

Currently, this program sets up the interrupt vector and launches our main entry point.
Then we output through UART0 (Serial Port) our string followed by a newline.

After that, we end up in an infinite loop. (Is that avoidable?)

Compiling
=========

First, you'll need the arm-none-eabi toolchain, which I won't cover installing here.

After having that, you'll need to download these sources. Then, the following commands should suffice:

    
    arm-none-eabi-as -mcpu=arm926ej-s startup.s -o startup.o
    arm-none-eabi-as -mcpu=arm926ej-s test.s -o test.o
    arm-none-eabi-ld -T test.ld startup.o test.o -o test.elf
    arm-none-eabi-objcopy -O binary test.elf test.bin

Executing
=========

You can execute the either test.elf or test.bin as:

    $ qemu-system-arm -machine versatilepb -nographic -kernel test.bin

Then you might have to kill the qemu process. ;)

