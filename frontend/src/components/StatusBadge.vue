<script setup>
import { computed } from 'vue'
import { STATUS_LABELS } from '../api/client'

const props = defineProps({
  status: { type: Number, required: true }
})

const label = computed(() => STATUS_LABELS[props.status] ?? 'Unknown')

const classes = computed(() => {
  switch (props.status) {
    case 0:
      return 'bg-sky-50 text-sky-800 ring-sky-200'
    case 1:
      return 'bg-amber-50 text-amber-800 ring-amber-200'
    case 2:
      return 'bg-tide-50 text-tide-800 ring-tide-200'
    case 3:
      return 'bg-ink-100 text-ink-600 ring-ink-200'
    default:
      return 'bg-ink-100 text-ink-600 ring-ink-200'
  }
})
</script>

<template>
  <span
    class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset"
    :class="classes"
  >
    {{ label }}
  </span>
</template>
