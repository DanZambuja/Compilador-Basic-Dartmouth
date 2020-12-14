ENTRY(_start)
SECTIONS
{
  .
  = 0x0;
  .text : { * (.text); }
  .data : {*(.data)}
	.bss : {*(.bss)}
  
  . = ALIGN(8);
  . = . + 0x1000;
  stack_top = .;
  . = . + 0x1000;
  irq_stack_top = .;
  . = . + 0x1000;
}
