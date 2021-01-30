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
LABEL_20:
   ldr r0, =1
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =10
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =30
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   b .
mem:
 .space 12


	.section	.rodata
	.global	UART0DR
UART0DR:
	.word	0x101F1000 @ This is the UART0 address on a versatileboard
	.section	.rodata.str1.4,"aMS",%progbits,1

