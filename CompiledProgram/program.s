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
   ldr r0, =PRINT_1
   bl print_uart0
   ldr r0, =PRINT_2
   bl print_uart0
LABEL_20:
    b LABEL_40
LABEL_30:
   ldr r0, =PRINT_3
   bl print_uart0
LABEL_40:
   ldr r0, =PRINT_4
   bl print_uart0
LABEL_50:
LABEL_60:
    b LABEL_80
LABEL_70:
   ldr r0, =PRINT_5
   bl print_uart0
LABEL_80:
   ldr r0, =PRINT_6
   bl print_uart0
	b	.

	.section	.rodata
	.global	UART0DR
UART0DR:
	.word	0x101F1000 @ This is the UART0 address on a versatileboard
	.section	.rodata.str1.4,"aMS",%progbits,1

PRINT_1: .ascii "HEY\012\000"
PRINT_2: .ascii "HELLO\012\000"
PRINT_3: .ascii "WILLNOTSHOW1\012\000"
PRINT_4: .ascii "WILLSHOW1\012\000"
PRINT_5: .ascii "WILLNOTSHOW2\012\000"
PRINT_6: .ascii "WILLSHOW2\012\000"
