	.cpu arm926ej-s
	.fpu softvfp
	.text
	.global	print_uart0
print_uart0:
	@ This routine allows us to print onto UART0DR
	@ We use r0 as the mem. address for our char*
	ldrb	r3, [r0, #0]	@ zero_extendqisi2
	cmp	r3, #0
	bxeq	lr
	ldr	r2, =UART0DR
	ldr	r2, [r2, #0]
print_uart0_loop:
	str	r3, [r2, #0]
	ldrb	r3, [r0, #1]!	@ zero_extendqisi2
	cmp	r3, #0
	bne	print_uart0_loop
	bx	lr

	.global	c_entry
	@ This is our entry point from startup.s
c_entry:
LABEL_10:
   ldr r11, =4
   ldr r12, =25
   add r0, r12, r11
   mov r12, r0
   ldr r11, =7
   sub r0, r12, r11
   mov r12, r0
   ldr r11, =4
   mul r0, r12, r11
   mov r12, r0
   ldr r10, =10
   ldr r11, =4
   add r0, r11, r10
   mov r11, r0
   add r0, r12, r11
   mov r12, r0
   ldr r11, =15
   add r0, r12, r11
   mov r12, r0
   ldr r9, =5
   ldr r10, =9
   mul r0, r10, r9
   mov r10, r0
   ldr r11, =3
   mul r0, r11, r10
   mov r11, r0
   mov r0, r12
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   b .
mem:
 .space 4


	.section	.rodata
	.global	UART0DR
UART0DR:
	.word	0x101F1000 @ This is the UART0 address on a versatileboard
	.section	.rodata.str1.4,"aMS",%progbits,1

