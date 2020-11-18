arm-none-eabi-objcopy -I binary -O elf32-littlearm -B arm image.bmp build/image.o

arm-none-eabi-as -mcpu=arm926ej-s irq.s -o build/irq.o
arm-none-eabi-gcc -mcpu=arm926ej-s -marm -g -c handler.c -o build/handler.o
arm-none-eabi-gcc -mcpu=arm926ej-s -marm -g -c uart.c -o build/uart.o

arm-none-eabi-gcc -mcpu=arm926ej-s -marm -g -c vid.c -o build/vid.o -std=c99
arm-none-eabi-as -mcpu=arm926ej-s font.S -o build/font.o
arm-none-eabi-gcc -mcpu=arm926ej-s -marm -g -c char12x16.c -o build/char12x16.o -std=c99

arm-none-eabi-ld build/irq.o build/handler.o build/uart.o build/image.o build/vid.o build/font.o build/char12x16.o libs/libgcc.a -T irqld.ld -o build/irq.elf

arm-none-eabi-objcopy -O binary build/irq.elf build/irq.bin

qemu-system-arm -s -M versatilepb -cpu arm926 -kernel build/irq.bin -serial telnet:localhost:1122,server -S


b task
b do_software_interrupt
b print_and_draw
b p_d_syscall_interrupt
b do_irq_interrupt
b p_d_syscall