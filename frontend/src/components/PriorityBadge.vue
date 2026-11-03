<script setup>
import { computed } from 'vue'
import { PRIORITY_LABELS } from '../api/client'

const props = defineProps({
  priority: { type: Number, required: true }
})

const label = computed(() => PRIORITY_LABELS[props.priority] ?? 'Unknown')

const classes = computed(() => {
  switch (props.priority) {
    case 2:
      return 'bg-rose-50 text-rose-800 ring-rose-200'
    case 1:
      return 'bg-orange-50 text-orange-800 ring-orange-200'
    default:
      return 'bg-emerald-50 text-emerald-800 ring-emerald-200'
  }
})
</script>

<template>
  <span
    class="inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset"
    :class="classes"
  >
    <span
      class="h-1.5 w-1.5 rounded-full"
      :class="{
        'bg-rose-500': priority === 2,
        'bg-orange-500': priority === 1,
        'bg-emerald-500': priority === 0
      }"
    />
    {{ label }}
  </span>
</template>
